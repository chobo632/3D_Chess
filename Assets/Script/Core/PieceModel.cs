using UnityEngine;

namespace Chess.Core
{
    public class PieceModel
    {
        // 共通情報
        public PieceType PieceType { get; }
        public PieceColor PieceColor { get; }
        

        // 
        public PieceModel(PieceType pieceType, PieceColor pieceColor)
        {
            PieceType  = pieceType;
            PieceColor = pieceColor;
        }

        public int GetForwardDirection()
        {
            return PieceColor == PieceColor.White ? -1 : 1;
        }
    }
    // 駒の種類
    public enum PieceType
    {
        None,
        Pawn,
        Rook,
        Knight,
        Bishop,
        Queen,
        King
    }
    // 駒の色（敵、味方）
    public enum PieceColor
    {
        None,
        White,
        Black
    }
    // ゲームモードの種類
    public enum GameMode
    {
        Normal,
        Random,
        Battle,
        Custom
    }
    // ゲームの勝利者判定
    public enum GameResult
    {
        WhiteWin,
        BlackWin,
        Draw
    }
}