using Chess.Core;
using Chess.Rules;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.GamePlay
{
    // 合法手の中からランダムに1手選ぶ
    public class AIPlayer
    {
        private readonly PieceColor aiColor;
        private readonly GameModeBase gameModeBase;
        private readonly BoardModel board;

        public AIPlayer(PieceColor aiColor, GameModeBase gameModeBase, BoardModel board)
        {
            this.aiColor = aiColor;
            this.gameModeBase = gameModeBase;
            this.board = board;
        }

        // AIの手を選んで返す（動かす駒と移動先）
        public (PieceModel piece, Vector2Int pos)? SelectMove()
        {
            var pieces = board.GetPieces(aiColor);
            var candidates = new List<(PieceModel piece, Vector2Int pos)>();

            foreach (var piece in pieces)
            {
                // RandomModeの場合は抽選された駒のみ対象
                if (gameModeBase is RandomMode randomMode)
                {
                    if (!randomMode.IsSelected(piece)) continue;
                }

                var legalMoves = gameModeBase.GetLegalMoves(piece, board.Model, board);

                foreach (var move in legalMoves)
                {
                    candidates.Add((piece, move));
                }
            }

            if (candidates.Count == 0) return null;

            int index = Random.Range(0, candidates.Count);
            return candidates[index];
        }
    }
}