using System.Collections.Generic;
using UnityEngine;
using Chess.Core;

namespace Chess.Rules
{
    public class NormalMode : GameModeBase
    {
        //
        private PieceModel enPassantTarget;

        // 合法手取得（キャスリングを追加）
        public override List<Vector2Int> GetLegalMoves(PieceModel piece, CellModel cellModel, BoardModel boardModel)
        {
            var legalMoves = base.GetLegalMoves(piece, cellModel, boardModel);

            // キングのみキャスリングを追加
            if (piece.PieceType == PieceType.King)
            {
                legalMoves.AddRange(GetCastlingMoves(piece, cellModel, boardModel));
            }

            return legalMoves;
        }

        // アンパッサン
        public override PieceModel GetEnPassantTarget()
        {
            return enPassantTarget;
        }
        // 
        public override void SetEnPassantTarget(PieceModel pawn)
        {
            enPassantTarget = pawn;
        }

        // キャスリング合法手取得
        private List<Vector2Int> GetCastlingMoves(PieceModel king, CellModel cellModel, BoardModel boardModel)
        {
            var moves = new List<Vector2Int>();
            bool kingMoved = boardModel.GetController(king)?.IsMoved ?? true;

            if (kingMoved)
            {
                return moves;
            }

            if (IsCheck(king.PieceColor, cellModel, boardModel)) 
            { 
                return moves;
            }

            var kingPos = cellModel.GetPosition(king);
            int y = kingPos.y;

            // キングサイド（右）
            TryAddCastling(king, kingPos, new Vector2Int(0, y), new Vector2Int(1, y), new Vector2Int(2, y), cellModel,boardModel, moves);
            // クイーンサイド（左）
            TryAddCastling(king, kingPos, new Vector2Int(7, y), new Vector2Int(5, y), new Vector2Int(4, y), cellModel,boardModel, moves);

            return moves;
        }

        private void TryAddCastling(PieceModel king, Vector2Int kingPos, Vector2Int rookPos, Vector2Int kingDest, Vector2Int rookDest, CellModel cellModel,BoardModel boardModel, List<Vector2Int> moves)
        {
            var rook = cellModel.GetPiece(rookPos);

            if (rook == null || rook.PieceType != PieceType.Rook)
            {
                return;
            }

            bool rookMoved = boardModel.GetController(rook)?.IsMoved ?? true;

            if (rookMoved)
            {
                return;
            }

            // キングとルークの間が空いているか
            int minX = Mathf.Min(kingPos.x, rookPos.x);
            int maxX = Mathf.Max(kingPos.x, rookPos.x);

            for (int x = minX + 1; x < maxX; x++)
            {
                if (cellModel.GetPiece(new Vector2Int(x, kingPos.y)) != null)
                {
                    return;
                }
            }

            // 通過マスがチェックされていないか
            var captured = cellModel.MovePiece(king, rookDest);
            bool isCheck = IsCheck(king.PieceColor, cellModel, boardModel);
            cellModel.UndoMove(king, captured, kingPos, rookDest);

            if (isCheck)
            {
                return;
            }

            // 移動先がチェックされていないか
            captured = cellModel.MovePiece(king, kingDest);
            isCheck  = IsCheck(king.PieceColor, cellModel, boardModel);
            cellModel.UndoMove(king, captured, kingPos, kingDest);

            if (isCheck)
            {
                return;
            }

            moves.Add(kingDest);
        }
    }
}
