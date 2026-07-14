using System.Collections.Generic;
using Chess.Core;
using Chess.Rules;

namespace Chess.GamePlay
{
    // RandomModeの抽選・再抽選フローを取りまとめる
    public class LotteryCoordinator
    {
        private readonly BoardModel board;

        public LotteryCoordinator(BoardModel board)
        {
            this.board = board;
        }

        // 手番開始時の抽選
        public (List<PieceModel> pieces, int remainingRerolls) Lottery(RandomMode randomMode, PieceColor color)
        {
            var pieces = randomMode.LotteryPiece(color, board.Model, board);
            return (pieces, randomMode.GetRemainingRerolls(color));
        }

        // 再抽選（上限到達・連続リロール済みならnull）
        public (List<PieceModel> pieces, int remainingRerolls)? Reroll(RandomMode randomMode, PieceColor color)
        {
            if (!randomMode.CanReroll(color))
            {
                return null;
            }

            randomMode.RerollPieces(color, board.Model, board);
            return (randomMode.GetSelectPieces(), randomMode.GetRemainingRerolls(color));
        }
    }
}
