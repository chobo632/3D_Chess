using System.Collections.Generic;
using UnityEngine;
using Chess.Core;

namespace Chess.UI
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] private CointossPanel cointossPanel;
        [SerializeField] private PausePanel pausePanel;
        [SerializeField] private PromotionPanel promotionPanel;
        [SerializeField] private RandomModePanel randomModePanel;
        [SerializeField] private GameStatusPanel gameStatusPanel;
        
        public static GameUIManager Instance;


        private void Awake()
        {
            Instance = this;
        }

        public void ShowCointoss(bool show) => cointossPanel.ShowCointoss(show);
        public void SetCointossResult(string text) => cointossPanel.SetCointossResult(text);
        public void ShowPause(bool show) => pausePanel.ShowPause(show);
        public void ShowPromotion(PieceModel piece) => promotionPanel.ShowPromotion(piece);
        public void UpdateTurnText(int turnCount) => gameStatusPanel.UpdateTurnText(turnCount);
        public void ShowCheckText(bool show) => gameStatusPanel.ShowCheckText(show);
        public void ShowRandomMode(bool show) => randomModePanel.ShowRandomMode(show);
        public void ShowLotteryPieces(List<PieceModel> pieces) => randomModePanel.ShowLotteryPieces(pieces);
        public void UpdateRerollText(int count) => randomModePanel.UpdateRerollText(count);
    }
}
