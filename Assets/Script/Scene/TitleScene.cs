using UnityEngine;
using UnityEngine.UI;
using Chess.Core;

namespace Chess.Scene
{
    public class TitleScene : MonoBehaviour
    {
        [SerializeField] CanvasGroup titlePanel;
        [SerializeField] CanvasGroup modePanel;
        // Modeパネル表示時に背景を暗くする
        [SerializeField] Image titleImage;

        // Mode選択画面に移行
        public void OnClickMode()
        {
            titleImage.color = new Color(1, 1, 1, 0.3f);
            ShowPanel(modePanel);
        }
        // Exitボタン
        public void OnClickExit()
        {
            Application.Quit();
        }

        // NormalModeのGameSceneに移行
        public void OnClickNormal()
        {
            SceneController.Instance.selectMode = GameMode.Normal;
            SceneController.Instance.ChangeScene("GameScene");
        }
        // RandomModeのGameSceneに移行
        public void OnClickRandom()
        {
            SceneController.Instance.selectMode = GameMode.Random;
            SceneController.Instance.ChangeScene("GameScene");
        }
        // Mode選択画面からTitle画面に戻る
        public void OnClickBack()
        {
            titleImage.color = new Color(1, 1, 1, 1f);
            ShowPanel(titlePanel);
        }

        // パネル切り替え共通処理
        private void ShowPanel(CanvasGroup panel)
        {
            // 全パネルを隠す
            titlePanel.alpha = 0;
            titlePanel.interactable = false;
            titlePanel.blocksRaycasts = false;

            modePanel.alpha = 0;
            modePanel.interactable = false;
            modePanel.blocksRaycasts = false;

            // 指定パネルだけ表示
            panel.alpha = 1;
            panel.interactable = true;
            panel.blocksRaycasts = true;
        }
    }
}

