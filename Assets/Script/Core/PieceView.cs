using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Chess.Core
{
    public class PieceView : MonoBehaviour
    {
        [SerializeField] private Slider hpBar;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private GameObject hpCanvas;

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

        // HPを更新
        public void UpdateHP(int currentHP, int maxHP)
        {
            if (hpCanvas == null) return;

            hpCanvas.SetActive(true);
            if (hpBar != null)
            {
                hpBar.value = (float)currentHP / maxHP;
            }
            if (hpText != null)
            {
                hpText.text = $"{currentHP}/{maxHP}";
            }
        }

        // HP表示を非表示（BattleMode以外用）
        public void HideHP()
        {
            if (hpCanvas != null) hpCanvas.SetActive(false);
        }
    }
}
