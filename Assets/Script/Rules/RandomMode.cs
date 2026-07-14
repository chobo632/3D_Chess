using System.Collections.Generic;
using UnityEngine;
using Chess.Core;

namespace Chess.Rules
{
    public class RandomMode : GameModeBase
    {
        // 
        private List<PieceModel> selectPieces = new();

        private const int MaxReroll = 3;
        private const int LotteryCount = 3;
        
        private int rerollCountWhite = 0;
        private int rerollCountBlack = 0;
        private bool isRerolled = false;

        private List<PieceModel> CreateLotteryPieces(PieceColor pieceColor, CellModel cellModel, BoardModel boardModel)
        {
            var lineup = new List<PieceModel>();

            foreach (var piece in cellModel.GetPieces(pieceColor))
            {
                if (GetLegalMoves(piece, cellModel, boardModel).Count > 0)
                {
                    lineup.Add(piece);
                }
            }

            if (lineup.Count <= LotteryCount)
            {
                return lineup;
            }

            var selectedPieces = new List<PieceModel>();
            var pool = new List<PieceModel>(lineup);

            for (int i = 0; i < LotteryCount; i++)
            {
                int index = Random.Range(0, pool.Count);
                selectedPieces.Add(pool[index]);
                pool.RemoveAt(index);
            }

            return selectedPieces;
        }

        // 手番開始時に駒を抽選
        public List<PieceModel> LotteryPiece(PieceColor pieceColor, CellModel cellModel, BoardModel boardModel)
        {
            selectPieces = CreateLotteryPieces(pieceColor, cellModel, boardModel);

            return selectPieces;
        }

        // 再抽選
        public List<PieceModel> RerollPieces(PieceColor pieceColor, CellModel cellModel, BoardModel boardModel)
        {
            if (!CanReroll(pieceColor))
            {
                return selectPieces;
            }

            RerollCount(pieceColor);
            isRerolled = true;
            selectPieces = CreateLotteryPieces(pieceColor, cellModel, boardModel);

            return selectPieces;
        }

        // 
        private int GetRerollCount(PieceColor color)
        {
            return color == PieceColor.White ? rerollCountWhite : rerollCountBlack;
        }

        // 
        private void RerollCount(PieceColor color)
        {
            if (color == PieceColor.White)
            {
                rerollCountWhite++;
            }
            else
            {
                rerollCountBlack++;
            }
        }

        // 
        public List<PieceModel> GetSelectPieces()
        {
            return selectPieces;
        }

        // 駒移動完了時に呼ぶ（次ターン用にリセット）
        public override void MoveComplet(PieceColor color)
        {
            isRerolled = false;
            selectPieces.Clear();
        }

        // 選ばれた駒か判定
        public bool IsSelected(PieceModel piece)
        {
            return selectPieces.Contains(piece);
        }

        // 
        public int GetRemainingRerolls(PieceColor color)
        {
            return MaxReroll - GetRerollCount(color);
        }

        public bool CanReroll(PieceColor color)
        {
            return GetRerollCount(color) < MaxReroll && !isRerolled;
        }
    }
}

