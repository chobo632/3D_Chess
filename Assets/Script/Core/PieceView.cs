using UnityEngine;

namespace Chess.Core
{
    public class PieceView : MonoBehaviour
    {
        private Renderer pieceRenderer;
        public  PieceModel Piece { get; private set; }

        public bool IsMoved { get; set; }

        // 
        private void Awake()
        {
            pieceRenderer = GetComponentInChildren<Renderer>();
        }

        // データと紐づけ
        public void Initialize(PieceModel piece)
        {
            Piece = piece;
            SetColor(piece.PieceColor);
        }

        // 色の設定
        public void SetColor(PieceColor pieceColor)
        {
            if (pieceColor == PieceColor.White)
            {
                pieceRenderer.material.color = Color.white;
            }
            else
            {
                pieceRenderer.material.color = Color.black;
            }
        }

        // 
        public void SetHighlightColor(Color color)
        {
            pieceRenderer.material.color = color;
        }

        // 
        public Color GetCurrentColor()
        {
            return pieceRenderer.material.color;
        }

        // 
        public void MoveTo(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }

        // 
        public void Lift(Vector3 worldPosition, float height)
        {
            transform.position = worldPosition + new Vector3(0, height, 0);
        }
    }
}
