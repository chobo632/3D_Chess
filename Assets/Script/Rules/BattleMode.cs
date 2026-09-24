using Chess.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Rules
{
    public class BattleMode : GameModeBase
    {
        public Action<PieceModel, int, int> HPChanged;

        // 各駒のステータス
        private readonly Dictionary<PieceModel, BattleStats> stats = new();
        // KingのHP回復カウント（自分のターン基準で何ターン被弾なしか）
        private readonly Dictionary<PieceModel, int> kingHealCounter = new();

        private int knightReactCount = 0;
        private const int MaxReactCount = 2;

        // 駒登録（ゲーム開始時・プロモーション時に呼ぶ）
        public void RegisterPiece(PieceModel piece)
        {
            stats[piece] = BattleStats.CreateDefault(piece.PieceType);

            if (piece.PieceType == PieceType.King)
            {
                kingHealCounter[piece] = 0;
            }
        }

        // プロモーション時：変化後のHPから20引いた状態でスタート
        public void RegisterPromotedPiece(PieceModel piece)
        {
            var defaultStats = BattleStats.CreateDefault(piece.PieceType);
            int startHP = Mathf.Max(1, defaultStats.MaxHP - 20);
            stats[piece] = new BattleStats(defaultStats.MaxHP, defaultStats.ATK, startHP);
        }

        public BattleStats GetStats(PieceModel piece)
        {
            stats.TryGetValue(piece, out var s);
            return s;
        }

        // 駒の削除（ステータスも同時に削除）
        public void RemovePiece(PieceModel piece, BoardModel boardModel, CellModel cellModel)
        {
            boardModel.RemovePiece(cellModel.GetPosition(piece));
            stats.Remove(piece);
            kingHealCounter.Remove(piece);
        }

        // 通常攻撃処理
        // 返り値：撃破したか
        public bool Attack(PieceModel attacker, PieceModel defender, CellModel cellModel, BoardModel boardModel)
        {
            var attackerStats = GetStats(attacker);
            var defenderStats = GetStats(defender);

            if (attackerStats == null || defenderStats == null)
            { 
                return false; 
            }

            // Rookの肩代わりチェック（Queen範囲攻撃は対象外）
            var guardRook = FindAdjacentRook(defender, cellModel);

            if (guardRook != null)
            {
                var rookStats = GetStats(guardRook);
                if (rookStats != null)
                {
                    rookStats.TakeDamage(attackerStats.ATK);
                    if (rookStats.IsDefeated)
                    {
                        RemovePiece(guardRook, boardModel, cellModel);
                    }
                }
                // 肩代わりされたのでdefenderは無傷、撃破なし
                return false;
            }

            // 通常ダメージ
            defenderStats.TakeDamage(attackerStats.ATK);
            HPChanged?.Invoke(defender, defenderStats.CurrentHP, defenderStats.MaxHP);

            // Kingが被弾したらカウントリセット
            if (defender.PieceType == PieceType.King && kingHealCounter.ContainsKey(defender))
            {
                kingHealCounter[defender] = 0;
            }

            return defenderStats.IsDefeated;
        }

        // 隣接するRookを探す（肩代わり判定用）
        // ※ Rook同士が隣接している場合は最初に見つかった方（TODO:プレイヤー選択UI）
        private PieceModel FindAdjacentRook(PieceModel target, CellModel cellModel)
        {
            var pos = cellModel.GetPosition(target);
            var neighbors = new Vector2Int[]
            {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                new(1, 1), new(1, -1), new(-1, 1), new(-1, -1)
            };

            foreach (var dir in neighbors)
            {
                var piece = cellModel.GetPiece(pos + dir);

                if (piece != null && piece.PieceType == PieceType.Rook && piece.PieceColor == target.PieceColor)
                {
                    return piece;
                }
            }

            return null;
        }

        // 再行動可能かチェック
        public bool CanReact(PieceModel piece)
        {
            return piece.PieceType == PieceType.Knight && knightReactCount < MaxReactCount;
        }

        // 再行動カウントを増やす
        public void AddReactCount()
        {
            knightReactCount++;
        }

        // 再行動カウントをリセット（ターン終了時に呼ぶ）
        public void ResetReactCount()
        {
            knightReactCount = 0;
        }

        // 
        public List<PieceModel> ExecuteBishopMove(PieceModel bishop, Vector2Int destination, CellModel cellModel, BoardModel boardModel)
        {
            var defeated = new List<PieceModel>();
            var from = cellModel.GetPosition(bishop);

            int dx = System.Math.Sign(destination.x - from.x);
            int dy = System.Math.Sign(destination.y - from.y);

            int pierceCount = 0;
            var current = from + new Vector2Int(dx, dy);

            // 移動先の1つ手前まで経路をチェック（貫通ダメージ）
            while (current != destination)
            {
                var pathPiece = cellModel.GetPiece(current);

                if (pathPiece != null && pathPiece.PieceColor != bishop.PieceColor)
                {
                    int pierceDamage = pierceCount == 0 ? 20 : 10;
                    var pathStats = GetStats(pathPiece);

                    if (pathStats != null)
                    {
                        pathStats.TakeDamage(pierceDamage);
                        HPChanged?.Invoke(pathPiece, pathStats.CurrentHP, pathStats.MaxHP);

                        if (pathPiece.PieceType == PieceType.King && kingHealCounter.ContainsKey(pathPiece))
                        {
                            kingHealCounter[pathPiece] = 0;
                        }

                        if (pathStats.IsDefeated)
                        {
                            defeated.Add(pathPiece);
                        }

                        pierceCount++;
                    }
                }

                current += new Vector2Int(dx, dy);
            }

            // 経路上で撃破した駒を除外
            foreach (var piece in defeated)
            {
                RemovePiece(piece, boardModel, cellModel);
            }

            // 移動先の敵に10ダメージ（撃破できる場合のみここに来る）
            var destPiece = cellModel.GetPiece(destination);
            if (destPiece != null && destPiece.PieceColor != bishop.PieceColor)
            {
                var destStats = GetStats(destPiece);
                if (destStats != null)
                {
                    destStats.TakeDamage(10);
                    HPChanged?.Invoke(destPiece, destStats.CurrentHP, destStats.MaxHP);

                    if (destPiece.PieceType == PieceType.King && kingHealCounter.ContainsKey(destPiece))
                    {
                        kingHealCounter[destPiece] = 0;
                    }

                    if (destStats.IsDefeated)
                    {
                        defeated.Add(destPiece);
                        RemovePiece(destPiece, boardModel, cellModel);
                    }
                }
            }

            return defeated;
        }

        // Queen範囲攻撃（移動後に周囲8マスの敵へ10ダメージ）
        // 返り値：撃破された駒のリスト
        public List<PieceModel> QueenSplash(PieceModel queen, CellModel cellModel, BoardModel boardModel)
        {
            var defeated = new List<PieceModel>();

            if (queen.PieceType != PieceType.Queen)
            { 
                return defeated;
            }

            var pos = cellModel.GetPosition(queen);
            var neighbors = new Vector2Int[]
            {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
                new(1, 1), new(1, -1), new(-1, 1), new(-1, -1)
            };

            foreach (var dir in neighbors)
            {
                var target = cellModel.GetPiece(pos + dir);

                if (target == null || target.PieceColor == queen.PieceColor)
                {
                    continue;
                }

                var targetStats = GetStats(target);

                if (targetStats == null)
                {
                    continue;
                }

                // Queen範囲攻撃はRookの肩代わり対象外
                targetStats.TakeDamage(10);
                HPChanged?.Invoke(target, targetStats.CurrentHP, targetStats.MaxHP);

                if (target.PieceType == PieceType.King && kingHealCounter.ContainsKey(target))
                {
                    kingHealCounter[target] = 0;
                }
                if (targetStats.IsDefeated)
                {
                    defeated.Add(target);
                }
            }

            // 撃破された駒を除外
            foreach (var piece in defeated)
            {
                RemovePiece(piece, boardModel, cellModel);
            }

            return defeated;
        }

        // Kingの回復カウント更新（自分のターン開始時に呼ぶ）
        public void UpdateKingRecovery(PieceColor color, CellModel cellModel)
        {
            var king = cellModel.GetKing(color);

            if (king == null || !kingHealCounter.ContainsKey(king))
            {
                return;
            }

            kingHealCounter[king]++;

            if (kingHealCounter[king] >= 2)
            {
                var kingStats = GetStats(king);
                kingStats?.Heal(20);
                if (kingStats != null)
                {
                    HPChanged?.Invoke(king, kingStats.CurrentHP, kingStats.MaxHP);
                }
                kingHealCounter[king] = 0;
            }
        }

        // KingのHPが0以下かチェック（勝利判定に使う）
        public bool IsKingDefeated(PieceColor color, CellModel cellModel)
        {
            var king = cellModel.GetKing(color);

            if (king == null)
            {
                return true;
            }

            var kingStats = GetStats(king);
            
            return kingStats == null || kingStats.IsDefeated;
        }

        // BattleModeではキャスリング・アンパッサンなし
        // チェック考慮なしの合法手を返す
        public override List<Vector2Int> GetLegalMoves(PieceModel piece, CellModel cellModel, BoardModel boardModel)
        {
            var moveAmount = new MoveAmount();
            if (piece.PieceType == PieceType.Bishop)
            {
                // BattleMode専用：stats情報を渡す
                return moveAmount.GetBishopBattleMoves(piece, cellModel, stats);
            }

            return moveAmount.GetMove(piece, cellModel, boardModel);
        }

        // BattleModeではチェック系は使わない
        public new bool IsCheck(PieceColor color, CellModel cellModel, BoardModel boardModel) => false;
        public new bool IsCheckmate(PieceColor color, CellModel cellModel, BoardModel boardModel) => false;
        public new bool IsStalemate(PieceColor color, CellModel cellModel, BoardModel boardModel) => false;
    }
}