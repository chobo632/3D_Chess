using UnityEngine;

namespace Chess.UI
{
    public class PausePanel : MonoBehaviour
    {
        // ポーズパネル
        [SerializeField] private CanvasGroup pausePanel;

        // Start is called before the first frame update
        void Start()
        {
            // 
            ShowPause(false);
        }

        // ポーズパネル表示
        public void ShowPause(bool show)
        {
            pausePanel.alpha = show ? 1 : 0;
            pausePanel.interactable = show;
            pausePanel.blocksRaycasts = show;
        }
    }

}

