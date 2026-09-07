using Chess.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.UI
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] private CointossPanel cointossPanel;
        [SerializeField] private PausePanel pausePanel;
        [SerializeField] private PromotionPanel promotionPanel;
        [SerializeField] private RandomModePanel randomModePanel;
        [SerializeField] private GameStatusPanel gameStatusPanel;
        [SerializeField] private PieceStatusPanel pieceStatusPanel;

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
        public void ShowRandomMode(bool show) => randomModePanel.ShowRandomMode(show);
        public void ShowLotteryPieces(List<PieceModel> pieces) => randomModePanel.ShowLotteryPieces(pieces);
        public void UpdateRerollText(int count) => randomModePanel.UpdateRerollText(count);
        public void ShowPieceStatus(PieceModel piece, BattleStats stats) => pieceStatusPanel.Show(piece, stats);
        public void HidePieceStatus() => pieceStatusPanel.Hide();
        public void ShowCheckText(PieceColor color) => gameStatusPanel.ShowCheckText(color);
        public void ShowKingHP(bool show) => gameStatusPanel.ShowKingHP(show);
        public void UpdateKingHP(PieceColor color, int current, int max) => gameStatusPanel.UpdateKingHP(color, current, max);
    }
}
