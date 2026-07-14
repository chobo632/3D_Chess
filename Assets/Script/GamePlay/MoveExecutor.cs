using UnityEngine;
using Chess.Core;
using Chess.Rules;

namespace Chess.GamePlay
{
    // 駒移動に関する特殊ルール担当
    public class MoveExecutor
    {
        private readonly BoardModel board;

        public MoveExecutor(BoardModel board)
        {
            this.board = board;
        }

        // 特殊ルール処理
        public void ExecuteMove(PieceModel piece, Vector2Int pos, GameModeBase gameModeBase)
        {
            HandleCapture(pos);
            HandleEnPassant(piece, pos, gameModeBase);

            var fromPos = board.Model.GetPosition(piece);
            HandleCastling(piece, pos);

            board.ExecuteMove(piece, pos);

            UpdateEnPassant(piece, fromPos, pos, gameModeBase);
        }

        // プロモーション
        public void Promotion(PieceModel piece, PieceType pieceType)
        {
            board.PromotePiece(piece, pieceType);
        }

        // 駒取得処理
        private void HandleCapture(Vector2Int pos)
        {
            var target = board.GetPiece(pos);

            if (target != null)
            {
                board.RemovePiece(pos);
            }
        }

        // アンパッサン
        private void HandleEnPassant(PieceModel piece, Vector2Int pos, GameModeBase gameModeBase)
        {
            if (piece.PieceType != PieceType.Pawn)
            {
                return;
            }

            int dir = -piece.GetForwardDirection();
            var enPassantPawn = board.GetPiece(new Vector2Int(pos.x, pos.y + dir));

            if (enPassantPawn != null && enPassantPawn == gameModeBase.GetEnPassantTarget())
            {
                board.RemovePiece(board.Model.GetPosition(enPassantPawn));
            }
        }

        // アンパッサン状態更新
        private void UpdateEnPassant(PieceModel piece, Vector2Int fromPos, Vector2Int pos, GameModeBase gameModeBase)
        {
            if (piece.PieceType == PieceType.Pawn && Mathf.Abs(pos.y - fromPos.y) == 2)
            {
                gameModeBase.SetEnPassantTarget(piece);
            }
            else
            {
                gameModeBase.SetEnPassantTarget(null);
            }
        }

        // キャスリング
        private void HandleCastling(PieceModel piece, Vector2Int pos)
        {
            if (piece.PieceType != PieceType.King)
            {
                return;
            }

            bool kingMoved = board.GetController(piece)?.IsMoved ?? true;

            if (kingMoved)
            {
                return;
            }

            var kingPos = board.Model.GetPosition(piece);
            int diff = pos.x - kingPos.x;

            if (Mathf.Abs(diff) != 2)
            {
                return;
            }

            int rookFromX = diff < 0 ? 0 : 7;
            int rookToX   = diff < 0 ? 2 : 4;
            var rook = board.GetPiece(new Vector2Int(rookFromX, pos.y));

            if (rook != null)
            {
                board.ExecuteMove(rook, new Vector2Int(rookToX, pos.y));
            }
        }
    }
}
