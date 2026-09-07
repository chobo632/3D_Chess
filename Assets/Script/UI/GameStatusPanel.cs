using UnityEngine;
using TMPro;
using Chess.Core;

namespace Chess.UI
{
    public class GameStatusPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private TextMeshProUGUI checkText;

        // BattleMode用King HP表示
        [SerializeField] private GameObject kingHPGroup;
        [SerializeField] private TextMeshProUGUI whiteKingHPText;
        [SerializeField] private TextMeshProUGUI blackKingHPText;

        void Start()
        {
            turnText.gameObject.SetActive(false);
            checkText.gameObject.SetActive(false);
            kingHPGroup.SetActive(false);
        }

        public void UpdateTurnText(int turnCount)
        {
            turnText.gameObject.SetActive(true);
            turnText.text = turnCount + "手目";
        }

        // 変更：bool → PieceColor
        public void ShowCheckText(PieceColor checkedColor)
        {
            if (checkedColor == PieceColor.None)
            {
                checkText.gameObject.SetActive(false);
                return;
            }

            checkText.gameObject.SetActive(true);
            checkText.text = checkedColor == PieceColor.White ? "White Check!" : "Black Check!";
        }

        // BattleModeのみ：King HP表示
        public void ShowKingHP(bool show)
        {
            kingHPGroup.SetActive(show);
        }

        public void UpdateKingHP(PieceColor color, int current, int max)
        {
            if (color == PieceColor.White)
            {
                whiteKingHPText.text = $"White King  {current}/{max}";
            }
            else
            {
                blackKingHPText.text = $"Black King  {current}/{max}";
            }
        }
    }
}