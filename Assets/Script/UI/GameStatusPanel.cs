using UnityEngine;
using TMPro;

namespace Chess.UI
{
    public class GameStatusPanel : MonoBehaviour
    {
        // ゲーム中UI
        [SerializeField] private TextMeshProUGUI turnText;
        [SerializeField] private TextMeshProUGUI checkText;

        // Start is called before the first frame update
        void Start()
        {
            turnText.gameObject.SetActive(false);
            checkText.gameObject.SetActive(false);
        }

        // ターンテキスト更新
        public void UpdateTurnText(int turnCount)
        {
            turnText.gameObject.SetActive(true);
            turnText.text = turnCount + "Turn";
        }

        // チェックテキスト表示
        public void ShowCheckText(bool show)
        {
            checkText.gameObject.SetActive(show);
        }
    }
}
