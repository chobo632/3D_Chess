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

    // プレイ人数の選択
    public enum PlayerCount
    {
        SoloPlay,    // 1人プレイ（AI対戦）
        DuoPlay      // 2人プレイ（分割画面）
    }
    // ゲームモードの種類
    public enum GameMode
    {
        Normal,
        Random,
        Battle,
        Custom
    }
    // 2人プレイ時の視点選択
    public enum CameraMode
    {
        AutoSwitch,  // 自動切り替え
        Overhead     // 俯瞰固定
    }

    // ゲームの勝利者判定
    public enum GameResult
    {
        WhiteWin,
        BlackWin,
        Draw
    }
}