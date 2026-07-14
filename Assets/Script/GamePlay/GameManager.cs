using System;
using System.Collections.Generic;
using UnityEngine;
using Chess.Core;
using Chess.Rules;

namespace Chess.GamePlay
{
    public class GameManager : MonoBehaviour
    {
        // 
        [SerializeField] BoardModel board;
        [SerializeField] private CameraController cameraController;

        public static GameManager Instance;

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

                    break;
                case GameMode.Custom:

                    break;
            }

            // モード変更後に再生成
            moveExecutor = new MoveExecutor(board);
            lotteryCoordinator = new LotteryCoordinator(board);
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

            moveExecutor.ExecuteMove(piece, pos, gameModeBase);

            if (JudgementPromotion(piece))
            {
                return;
            }

            EndTurn();
        }

        // 手番判定
        private bool CanMove(PieceModel piece)
        {
            return piece.PieceColor == currentTurn;
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

            cameraController?.SetViewByTurn(currentTurn);
            TurnChanged?.Invoke(turnCount);

            if (gameModeBase is RandomMode randomMode)
            {
                NotifyLotteryUpdate(lotteryCoordinator.Lottery(randomMode, currentTurn));
            }
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
    }
}
