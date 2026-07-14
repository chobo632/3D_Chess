using Chess.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Rules
{
    public abstract class GameModeBase
    {
        MoveAmount moveAmount = new MoveAmount();

        // 合法手取得
        public virtual List<Vector2Int> GetLegalMoves(PieceModel piece, CellModel cellModel, BoardModel boardModel)
        {
            var legalMoves = new List<Vector2Int>();
            // 疑似合法手
            var moves = moveAmount.GetMove(piece, cellModel, boardModel);

            foreach (var move in moves)
            {
                var from = cellModel.GetPosition(piece);
                var captured = cellModel.MovePiece(piece, move);
                bool isCheck = IsCheck(piece.PieceColor, cellModel, boardModel);
                cellModel.UndoMove(piece, captured, from, move);

                // チェックされないなら合法
                if (!isCheck)
                {
                    legalMoves.Add(move);
                }
            }

            return legalMoves;
        }

        // アンパッサン
        public virtual PieceModel GetEnPassantTarget()
        {
            return null;
        }

        // 
        public virtual void SetEnPassantTarget(PieceModel pawn) { }

        public virtual void MoveComplet(PieceColor color) { }

        // チェック判定
        public bool IsCheck(PieceColor color, CellModel cellModel, BoardModel boardModel)
        {
            var king    = cellModel.GetKing(color);
            var kingPos = cellModel.GetPosition(king);
            var enemy = color == PieceColor.White ? PieceColor.Black : PieceColor.White;
            var enemyPieces = cellModel.GetPieces(enemy);

            foreach (var piece in enemyPieces)
            {
                var moves = moveAmount.GetMove(piece, cellModel, boardModel);

                if (moves.Contains(kingPos))
                {
                    return true;
                }
            }

            return false;
        }

        // チェックメイト判定
        public bool IsCheckmate(PieceColor color, CellModel cellModel, BoardModel boardModel)
        {
            if (!IsCheck(color, cellModel, boardModel))
            {
                return false;
            }

            return !HasLegalMove(color, cellModel, boardModel);
        }

        // ステイルメイト判定
        public bool IsStalemate(PieceColor color, CellModel cellModel, BoardModel boardModel)
        {
            if (IsCheck(color, cellModel, boardModel))
            {
                return false;
            }

            return !HasLegalMove(color, cellModel, boardModel);
        }

        // ポーンプロモーション判定
        public bool IsPromotion(PieceModel piece, CellModel cellModel)
        {
            if (piece.PieceType != PieceType.Pawn)
            {
                return false;
            }

            int promotionRow = piece.PieceColor == PieceColor.White ? 0 : 7;

            return cellModel.GetPosition(piece).y == promotionRow;
        }

        // 合法手判定
        private bool HasLegalMove(PieceColor color, CellModel cellModel, BoardModel boardModel)
        {
            var pieces = cellModel.GetPieces(color);

            foreach (var piece in pieces)
            {
                if (GetLegalMoves(piece, cellModel, boardModel).Count > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
