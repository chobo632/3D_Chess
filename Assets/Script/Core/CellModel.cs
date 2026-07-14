using System.Collections.Generic;
using UnityEngine;

namespace Chess.Core
{
    // 盤面データの管理担当
    public class CellModel
    {
        private const int BoardArea_X = 8;
        private const int BoardArea_Y = 8;

        private readonly PieceModel[,] pieces = new PieceModel[BoardArea_X, BoardArea_Y];
        private readonly Dictionary<PieceModel, Vector2Int> piecePositions = new();

        // 駒を初期配置
        public void Place(PieceModel piece,Vector2Int pos)
        {
            pieces[pos.x, pos.y]  = piece;
            piecePositions[piece] = pos;
        }

        public Vector2Int GetPosition(PieceModel piece)
        {
            return piecePositions[piece];
        }

        public PieceModel GetPiece(Vector2Int pos)
        {
            if (IsOffBoard(pos))
            {
                return null;
            }

            return pieces[pos.x, pos.y];
        }
        
        public List<PieceModel> GetPieces(PieceColor color)
        {
            var result = new List<PieceModel>();

            for (int x = 0; x < BoardArea_X; x++)
            {
                for (int y = 0; y < BoardArea_Y; y++)
                {
                    var piece = pieces[x, y];

                    if (piece != null && piece.PieceColor == color)
                    {
                        result.Add(piece);
                    }
                }
            }

            return result;
        }

        public PieceModel GetKing(PieceColor color)
        {
            for (int x = 0; x < BoardArea_X; x++)
            {
                for (int y = 0; y < BoardArea_Y; y++)
                {
                    var piece = pieces[x, y];

                    if (piece != null && piece.PieceColor == color && piece.PieceType == PieceType.King)
                    {
                       return piece;
                    }
                }
            }

            return null;
        }

        // 移動先にあった駒を返す（駒取得）
        public PieceModel MovePiece(PieceModel piece,Vector2Int pos)
        {
            if (IsOffBoard(pos))
            {
                return null;
            }

            var from = piecePositions[piece];
            var captured = pieces[pos.x, pos.y];

            pieces[from.x, from.y] = null;
            pieces[pos.x, pos.y]  = piece;
            piecePositions[piece] = pos;

            if (captured != null)
            {
                piecePositions.Remove(captured);
            }

            return captured;
        }

        public void UndoMove(PieceModel piece, PieceModel captured, Vector2Int from, Vector2Int to)
        {
            pieces[from.x, from.y] = piece;
            pieces[to.x, to.y]  = captured;
            piecePositions[piece]  = from;

            if (captured != null)
            {
                piecePositions[captured] = to;
            }
        }

        // 駒を盤から取り除く
        public void ClearCell(Vector2Int pos)
        {
            var piece = pieces[pos.x, pos.y];

            if (piece != null)
            {
                piecePositions.Remove(piece);
                pieces[pos.x, pos.y] = null;
            }
        }

        public static bool IsOffBoard(Vector2Int pos)
        {
            return pos.x < 0 || pos.x >= BoardArea_X || pos.y < 0 || pos.y >= BoardArea_Y;
        }
    }
}
