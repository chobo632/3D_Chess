using Chess.Core;
using Chess.Rules;
using Chess.Scene;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.GamePlay
{
    public class GameManager : MonoBehaviour
    {
        // 
        [SerializeField] BoardModel board;
        [SerializeField] private CameraController cameraController;

        public static GameManager Instance;
        public Action<PieceModel> AttackFailed;

        // ターン・チェック・プロモーション・終了イベント
        public Action<int> TurnChanged;
        public Action<bool> CheckChanged;
        public Action<PieceModel> PromotionRequested;
        public Action<GameResult> GameEnd;

        // RandomMode関連イベント
        public Action RandomModeStarted;
        public Action<List<PieceModel>> LotteryPiecesUpdated;
        public Action<int> RerollCountUpdated;

        // コイントス結果イベント
        public Action<string> CointossResultReady;

        // 
        public PieceColor currentTurn = PieceColor.White;
        public int turnCount = 0;

        // 
        public bool IsPaused { get; set; }
        public bool IsCointoss { get; set; }
        public bool IsPromotion { get; private set; }

        // 特殊ルール実行
        private MoveExecutor moveExecutor;
        // コイントス
        private CointossService cointossService;
        // 抽選の担当
        private LotteryCoordinator lotteryCoordinator;

        // ゲームモード
        private GameModeBase gameModeBase;

        // 対戦相手AI
        private AIPlayer aiPlayer;
        private PieceColor aiColor;
        // AI駒移動の間
        private const float AIThinkDelay = 0.8f;   // 考える時間
        private const float AISelectDelay = 0.8f;  // 駒を選んでから動かすまでの時間

        // 
        private void Awake()
        {
            Instance = this;
            gameModeBase = new NormalMode();
            cointossService = new CointossService();
        }

        public PieceView GetController(PieceModel piece)
        {
            return board.GetController(piece);
        }

        // Modeをセット（GameSceneから呼ぶ）
        public void SetMode(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Normal:
                    gameModeBase = new NormalMode();
                    break;
                case GameMode.Random:
                    gameModeBase = new RandomMode();
                    break;
                case GameMode.Battle:
                    gameModeBase = new BattleMode();
                    break;
                case GameMode.Custom:

                    break;
            }

            // モード変更後に再生成
            moveExecutor = new MoveExecutor(board);
            lotteryCoordinator = new LotteryCoordinator(board);

            // BattleModeなら全駒のステータスを登録
            if (gameModeBase is BattleMode battleMode)
            {
                // 全駒登録
                foreach (var piece in board.GetPieces(PieceColor.White))
                {
                    battleMode.RegisterPiece(piece);
                    var stats = battleMode.GetStats(piece);
                    board.GetController(piece)?.UpdateHP(stats.CurrentHP, stats.MaxHP);
                }
                foreach (var piece in board.GetPieces(PieceColor.Black))
                {
                    battleMode.RegisterPiece(piece);
                    var stats = battleMode.GetStats(piece);
                    board.GetController(piece)?.UpdateHP(stats.CurrentHP, stats.MaxHP);
                }

                // HP変化時に表示更新
                battleMode.HPChanged += (piece, current, max) =>
                {
                    board.GetController(piece)?.UpdateHP(current, max);
                };
            }
        }

        // Playerのコイントス選択を保存
        public void SetPlayerChoice(bool choseHeadTail)
        {
            cointossService.SetPlayerChoice(choseHeadTail);
        }

        // コイントス実行
        public void ExecuteCointoss()
        {
            // 先行は白駒
            currentTurn = PieceColor.White;
            string message = cointossService.Execute();
            CointossResultReady?.Invoke(message);
        }

        // 駒移動要求
        public void MoveRequest(PieceModel piece, Vector2Int pos)
        {
            if (!CanMove(piece))
            {
                return;
            }

            if (gameModeBase is BattleMode battleMode)
            {
                ExecuteBattleMove(piece, pos, battleMode);
            }
            else
            {
                moveExecutor.ExecuteMove(piece, pos, gameModeBase);
                if (JudgementPromotion(piece)) return;
                EndTurn();
            }
        }

        // 手番判定
        private bool CanMove(PieceModel piece)
        {
            return piece.PieceColor == currentTurn;
        }

        // BattleMode専用の移動処理
        private void ExecuteBattleMove(PieceModel piece, Vector2Int pos, BattleMode battleMode)
        {
            var target = board.GetPiece(pos);

            if (target == null)
            {
                // 空マスへの通常移動
                board.ExecuteMove(piece, pos);

                // Queen範囲攻撃
                if (piece.PieceType == PieceType.Queen)
                {
                    battleMode.QueenSplash(piece, board.Model, board);
                }
            }
            else
            {
                // 敵駒への攻撃
                bool defeated = battleMode.Attack(piece, target, board.Model, board);

                if (defeated)
                {
                    // 撃破成功時敵を除外して攻撃側が前進
                    battleMode.RemovePiece(target, board, board.Model);
                    board.ExecuteMove(piece, pos);

                    // Queen範囲攻撃
                    if (piece.PieceType == PieceType.Queen)
                    {
                        battleMode.QueenSplash(piece, board.Model, board);
                    }

                    // TODO フェーズ2：Knight再行動
                }
                else
                {
                    // 撃破失敗時InputManagerに通知して駒を元の位置に戻させる
                    AttackFailed?.Invoke(piece);
                }
            }

            // BattleMode勝利判定
            if (CheckBattleGameEnd(battleMode)) return;

            // プロモーション判定（Pawnのみ）
            if (JudgementPromotion(piece)) return;

            EndTurn();            
        }

        // プロモーション判定
        private bool JudgementPromotion(PieceModel piece)
        {
            if (!gameModeBase.IsPromotion(piece, board.Model))
            {
                return false;
            }

            IsPromotion = true;
            PromotionRequested?.Invoke(piece);

            return true;
        }

        // ターン終了
        private void EndTurn()
        {
            gameModeBase.MoveComplet(currentTurn);
            ChangeTurn();
            CheckGameEnd();
        }

        // プロモーション完了時に呼ぶ
        public void ExecutePromotion(PieceModel piece, PieceType type)
        {
            moveExecutor.Promotion(piece, type);

            // BattleModeならプロモーション後の駒をステータス登録（HP - 20）
            if (gameModeBase is BattleMode battleMode)
            {
                var newPiece = board.GetPiece(board.Model.GetPosition(piece));

                if (newPiece != null)
                {
                    battleMode.RegisterPromotedPiece(newPiece);
                }
            }

                IsPromotion = false;
            ChangeTurn();
            CheckGameEnd();
        }

        // RandomModeの初回抽選
        public void StartLottery()
        {
            if (gameModeBase is RandomMode randomMode)
            {
                RandomModeStarted?.Invoke();
                NotifyLotteryUpdate(lotteryCoordinator.Lottery(randomMode, currentTurn));
            }
        }

        // 
        public List<Vector2Int> GetLegalMoves(PieceModel piece)
        {
            return gameModeBase.GetLegalMoves(piece, board.Model, board);
        }

        // ターン変更
        private void ChangeTurn()
        {
            if (currentTurn == PieceColor.Black)
            {
                turnCount++;
            }

            currentTurn = currentTurn == PieceColor.White ? PieceColor.Black : PieceColor.White;

            // BattleModeのみ：ターン開始時にKing回復カウント更新
            if (gameModeBase is BattleMode battleMode)
            {
                battleMode.UpdateKingRecovery(currentTurn, board.Model);
            }

            cameraController?.SetViewByTurn(currentTurn);
            TurnChanged?.Invoke(turnCount);

            if (gameModeBase is RandomMode randomMode)
            {
                NotifyLotteryUpdate(lotteryCoordinator.Lottery(randomMode, currentTurn));
            }

            // AIのターンなら自動で手を選んで実行
            StartCoroutine(TryAIMoveWithDelay());
        }
        
        // 再抽選
        public void RerollRequest()
        {
            if (gameModeBase is not RandomMode randomMode)
            {
                return;
            }

            var result = lotteryCoordinator.Reroll(randomMode, currentTurn);

            if (result == null)
            {
                return;
            }

            NotifyLotteryUpdate(result.Value);
        }

        // 抽選結果をUIに通知
        private void NotifyLotteryUpdate((List<PieceModel> pieces, int remainingRerolls) result)
        {
            LotteryPiecesUpdated?.Invoke(result.pieces);
            RerollCountUpdated?.Invoke(result.remainingRerolls);
        }

        // 抽選された駒
        public bool IsLotteryPiece(PieceModel piece)
        {
            if (gameModeBase is RandomMode randomMode)
            {
                return randomMode.IsSelected(piece);
            }
            // RandomMode以外は全駒選択可能
            return true;
        }

        // RandomModeを有効化
        public bool IsRandomModeActive()
        {
            return gameModeBase is RandomMode;
        }

        // エンド判定
        private void CheckGameEnd()
        {
            // チェックメイト判定
            if (gameModeBase.IsCheckmate(currentTurn, board.Model, board))
            {
                // 手番側が負け
                var result = currentTurn == PieceColor.White ? GameResult.BlackWin : GameResult.WhiteWin;
                GameEnd?.Invoke(result);
            }
            // ドロー判定
            else if (gameModeBase.IsStalemate(currentTurn, board.Model, board) || (IsKingOnly(PieceColor.White) && IsKingOnly(PieceColor.Black)))
            {
                GameEnd?.Invoke(GameResult.Draw);
            }
            // チェック判定表示
            else
            {
                CheckChanged?.Invoke(gameModeBase.IsCheck(currentTurn, board.Model, board));
            }
        }

        // キングが残っているか
        private bool IsKingOnly(PieceColor color)
        {
            List<PieceModel> pieces = board.GetPieces(color);
            return pieces.Count == 1 && pieces[0].PieceType == PieceType.King;
        }

        // BattleMode勝利判定
        private bool CheckBattleGameEnd(BattleMode battleMode)
        {
            // 50ターン経過で引き分け
            if (turnCount >= 50)
            {
                GameEnd?.Invoke(GameResult.Draw);
                return true;
            }

            // KingのHPが0以下になった側の負け
            if (battleMode.IsKingDefeated(PieceColor.White, board.Model))
            {
                GameEnd?.Invoke(GameResult.BlackWin);
                return true;
            }
            if (battleMode.IsKingDefeated(PieceColor.Black, board.Model))
            {
                GameEnd?.Invoke(GameResult.WhiteWin);
                return true;
            }

            return false;
        }

        // AIの色
        public void SetAIColor()
        {
            if (SceneController.Instance.playerCount != PlayerCount.SoloPlay)
            {
                return;
            }

            // プレイヤーが白ならAIは黒（その逆も同様）
            bool isPlayerWhite = cointossService.IsPlayerWhite();
            aiColor = isPlayerWhite ? PieceColor.Black : PieceColor.White;
            aiPlayer = new AIPlayer(aiColor, gameModeBase, board);

            // プレイヤーの色をカメラに伝えて視点を固定
            var playerColor = isPlayerWhite ? PieceColor.White : PieceColor.Black;
            cameraController?.SetPlayerColor(playerColor);
        }

        // AI実行

        private IEnumerator TryAIMoveWithDelay()
        {
            if (aiPlayer == null) yield break;
            if (currentTurn != aiColor) yield break;

            // 考える間
            yield return WaitFor(AIThinkDelay);

            // 手を選ぶ
            var result = aiPlayer.SelectMove();
            if (result == null) yield break;
            var (piece, pos) = result.Value;

            // 駒を選択して移動範囲を表示
            var controller = board.GetController(piece);
            var legalMoves = gameModeBase.GetLegalMoves(piece, board.Model, board);
            controller.Lift(board.GetWorldPosition(board.Model.GetPosition(piece)), 0.3f);
            board.ShowMoves(legalMoves);

            // 3. 選んだ状態でまた待機
            yield return WaitFor(AISelectDelay);

            board.HideMoves();
            MoveRequest(piece, pos);
        }

        // ポーズ中は時間を止める待機
        private IEnumerator WaitFor(float seconds)
        {
            float elapsed = 0f;
            while (elapsed < seconds)
            {
                if (!IsPaused && !IsPromotion)
                {
                    elapsed += Time.deltaTime;
                }
                yield return null;
            }
        }

        // ゲーム開始時にAIが先行なら実行（外部から呼ぶ用）
        public void TryAIMoveStart()
        {
            if (aiPlayer == null) return;
            if (currentTurn != aiColor) return;

            StartCoroutine(TryAIMoveWithDelay());
        }

        // AIの駒かどうかを判定
        public bool IsAIPiece(PieceModel piece)
        {
            if (SceneController.Instance.playerCount != PlayerCount.SoloPlay) return false;
            if (aiPlayer == null) return false;
            return piece.PieceColor == aiColor;
        }
    }
}
