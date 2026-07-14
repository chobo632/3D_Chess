using System.Collections.Generic;
using UnityEngine;

namespace Chess.Core
{
    public class MoveAmount
    {
        // 各駒の移動範囲設定
        public List<Vector2Int> GetMove(PieceModel piece, CellModel cellModel, BoardModel boardModel)
        {
            var moves = new List<Vector2Int>();
            var pos   = cellModel.GetPosition(piece);

            switch (piece.PieceType)
            {
                case PieceType.Rook:
                    // 上下左右にスライド
                    moves.AddRange(GetSlidingMoves(pos, new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }, piece.PieceColor, cellModel));
                    break;

                case PieceType.Bishop:
                    // 斜め4方向にスライド
                    moves.AddRange(GetSlidingMoves(pos, new Vector2Int[] { new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) }, piece.PieceColor, cellModel));
                    break;

                case PieceType.Queen:
                    // 8方向全てにスライド
                    moves.AddRange(GetSlidingMoves(pos, new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right, new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) }, piece.PieceColor, cellModel));
                    break;

                case PieceType.Knight:
                    // ナイト特有のL字ステップ
                    Vector2Int[] knightSteps =
                    { new(1, 2),
                      new(2, 1),
                      new(2, -1),
                      new(1, -2),
                      new(-1, -2),
                      new(-2, -1),
                      new(-2, 1),
                      new(-1, 2)
                    };

                    foreach (var step in knightSteps) AddIfValid(pos + step, piece.PieceColor, cellModel, moves);
                    break;

                case PieceType.King:
                    // 周囲1マス
                    Vector2Int[] kingSteps =
                    { Vector2Int.up,
                      Vector2Int.down,
                      Vector2Int.left,
                      Vector2Int.right,
                      new(1, 1),
                      new(1, -1),
                      new(-1, 1),
                      new(-1, -1)
                    };

                    foreach (var step in kingSteps) AddIfValid(pos + step, piece.PieceColor, cellModel, moves);
                    break;

                case PieceType.Pawn:
                    GetPawnMoves(piece, pos, cellModel,boardModel, moves);
                    break;
            }

            return moves;
        }

        // ポーン専用移動関数
        private void GetPawnMoves(PieceModel piece, Vector2Int pos, CellModel cellModel,BoardModel boardModel, List<Vector2Int> moves)
        {
            // 進行方向＆移動量
            int dir = piece.GetForwardDirection();
            var forward = pos + new Vector2Int(0, dir);
            var doubleForward = pos + new Vector2Int(0, dir * 2);
            var diagonallyR   = pos + new Vector2Int(1, dir);
            var diagonallyL   = pos + new Vector2Int(-1, dir);

            // 
            var frontPiece = cellModel.GetPiece(forward);
            var frontPieceDouble = cellModel.GetPiece(doubleForward);
            var targetPieceR = cellModel.GetPiece(diagonallyR);
            var targetPieceL = cellModel.GetPiece(diagonallyL);

            bool isMoved = boardModel.GetController(piece)?.IsMoved ?? false;

            // 通常ポーン移動
            if (frontPiece == null)
            {
                moves.Add(forward);

                if (!isMoved && frontPieceDouble == null)
                {
                    moves.Add(doubleForward);
                }
            }

            // 相手の駒が取れる位置にある時の移動
            if (targetPieceR != null && targetPieceR.PieceColor != piece.PieceColor)
            {
                moves.Add(diagonallyR);
            }
            if (targetPieceL != null && targetPieceL.PieceColor != piece.PieceColor)
            {
                moves.Add(diagonallyL);
            }
        }

        // 
        private List<Vector2Int> GetSlidingMoves(Vector2Int start, Vector2Int[] directions, PieceColor myColor, CellModel cellModel)
        {
            var moves = new List<Vector2Int>();

            foreach (var dir in directions)
            {
                for (int i = 1; i < 8; i++)
                {
                    var target = start + dir * i;

                    // 盤外なら終了
                    if (IsOffBoard(target))
                    break;

                    var targetPiece = cellModel.GetPiece(target);

                    if (targetPiece == null)
                    {
                        moves.Add(target);
                    }
                    else
                    {
                        if (targetPiece.PieceColor != myColor)
                        {
                            moves.Add(target);
                        }
                        break;
                    }
                }
            }

            return moves;
        }

        // 
        private void AddIfValid(Vector2Int target, PieceColor myColor, CellModel cellModel, List<Vector2Int> moves)
        {
            if (IsOffBoard(target))
            {
                return;
            }

            var targetPiece = cellModel.GetPiece(target);

            if (targetPiece == null || targetPiece.PieceColor != myColor)
            {
                moves.Add(target);
            }
        }

        // 
        private bool IsOffBoard(Vector2Int pos) => CellModel.IsOffBoard(pos);
    }
}
