using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Chess.Core;
using Chess.GamePlay;

namespace Chess.UI
{
    public class RandomModePanel : MonoBehaviour
    {
        // RandomMode専用UI
        [SerializeField] private CanvasGroup randomModePanel;
        [SerializeField] private TextMeshProUGUI rerollText;

        private List<PieceView> lotteryControllers = new();

        // Start is called before the first frame update
        void Start()
        {
            ShowRandomMode(false);
        }

        // 駒の再抽選
        public void OnClickReroll()
        {
            GameManager.Instance.RerollRequest();
        }

        // 残り再抽選回数更新
        public void UpdateRerollText(int count)
        {
            rerollText.text =  count + "";
        }

        // RandomModeパネル表示
        public void ShowRandomMode(bool show)
        {
            randomModePanel.alpha = show ? 1 : 0;
            randomModePanel.interactable = show;
            randomModePanel.blocksRaycasts = show;
        }

        // 抽選された駒をハイライト表示
        public void ShowLotteryPieces(List<PieceModel> pieces)
        {
            HideLotteryPieces();

            foreach (var piece in pieces)
            {
                var controller = GameManager.Instance.GetController(piece);

                if (controller != null)
                {
                    lotteryControllers.Add(controller);
                    controller.SetHighlightColor(new Color(0.5f, 0.5f, 1.0f, 1.0f));
                }
            }
        }
        // ハイライト解除
        public void HideLotteryPieces()
        {
            foreach (var controller in lotteryControllers)
            {
                if (controller != null)
                {
                    controller.SetColor(controller.Piece.PieceColor);
                }
            }

            lotteryControllers.Clear();
        }
    }
}
