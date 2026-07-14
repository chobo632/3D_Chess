using System.Collections.Generic;
using UnityEngine;
using Chess.Core;
using Chess.GamePlay;

namespace Chess.UI
{
    public class PromotionPanel : MonoBehaviour
    {
        // プロモーションパネル
        [SerializeField] private CanvasGroup promotionPanel;

        // 
        private PieceModel promotionPiece;

        // Start is called before the first frame update
        void Start()
        {
            // 
            promotionPanel.alpha = 0;
            promotionPanel.interactable = false;
            promotionPanel.blocksRaycasts = false;
        }

        // 完了通知
        private void CompletePromotion(PieceType type)
        {
            promotionPanel.alpha = 0;
            promotionPanel.interactable = false;
            promotionPanel.blocksRaycasts = false;
            GameManager.Instance.ExecutePromotion(promotionPiece, type);
            promotionPiece = null;
        }

        // 各プロモーションのOnClick
        public void OnClickPromoteQueen()
        {
            CompletePromotion(PieceType.Queen);
        }
        public void OnClickPromoteRook()
        {
            CompletePromotion(PieceType.Rook);
        }
        public void OnClickPromoteBishop()
        {
            CompletePromotion(PieceType.Bishop);
        }
        public void OnClickPromoteKnight()
        {
            CompletePromotion(PieceType.Knight);
        }

        // プロモーションパネル表示
        public void ShowPromotion(PieceModel piece)
        {
            promotionPiece = piece;
            promotionPanel.alpha = 1;
            promotionPanel.interactable = true;
            promotionPanel.blocksRaycasts = true;
        }
    }
}
