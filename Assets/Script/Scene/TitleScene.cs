using UnityEngine;
using UnityEngine.UI;
using Chess.Core;

namespace Chess.Scene
{
    public class TitleScene : MonoBehaviour
    {
        [SerializeField] private CanvasGroup titlePanel;
        [SerializeField] private CanvasGroup playerCountPanel;
        [SerializeField] private CanvasGroup modePanel;
        [SerializeField] private CanvasGroup cameraModePanel;
        // 背景の色を少し暗くする
        [SerializeField] private GameObject darkOverlay;

        // 人数選択画面に移行
        public void OnClickPlay()
        {
            darkOverlay.SetActive(true);
            ShowPanel(playerCountPanel);
        }
        // Exitボタン
        public void OnClickExit()
        {
            Application.Quit();
        }

        // Mode選択画面に移行
        public void OnClickSoloPlayer()
        {
            SceneController.Instance.playerCount = PlayerCount.SoloPlay;
            ShowPanel(modePanel);
        }
        // Mode選択画面に移行
        public void OnClickDuoPlayer()
        {
            SceneController.Instance.playerCount = PlayerCount.DuoPlay;
            ShowPanel(modePanel);
        }
        // 人数選択画面からTitle画面に戻る
        public void OnClickBackToTitle()
        {
            darkOverlay.SetActive(false);
            ShowPanel(titlePanel);
        }

        // NormalModeのGameSceneに移行or視点選択画面に移行
        public void OnClickNormal()
        {
            SceneController.Instance.selectMode = GameMode.Normal;
            // DuoPlayでゲームモード選択後に視点選択へ
            if (SceneController.Instance.playerCount == PlayerCount.DuoPlay)
            {
                ShowPanel(cameraModePanel);  // DuoPlayなら視点選択へ
            }
            else
            {
                SceneController.Instance.ChangeScene("GameScene");  // SoloPlayはそのまま
            }
        }
        // RandomModeのGameSceneに移行or視点選択画面に移行
        public void OnClickRandom()
        {
            SceneController.Instance.selectMode = GameMode.Random;
            // DuoPlayでゲームモード選択後に視点選択へ
            if (SceneController.Instance.playerCount == PlayerCount.DuoPlay)
            {
                ShowPanel(cameraModePanel);
            }
            else
            {
                SceneController.Instance.ChangeScene("GameScene");  // SoloPlayはそのまま
            }
        }
        // BattleModeのGameSceneに移行or視点選択画面に移行
        public void OnClickBattle()
        {
            SceneController.Instance.selectMode = GameMode.Battle;
            // DuoPlayでゲームモード選択後に視点選択へ
            if (SceneController.Instance.playerCount == PlayerCount.DuoPlay)
            {
                ShowPanel(cameraModePanel);
            }
            else
            {
                SceneController.Instance.ChangeScene("GameScene");  // SoloPlayはそのまま
            }
        }
        // Mode選択画面から人数選択画面に戻る
        public void OnClickBackToPlayerCount()
        {
            ShowPanel(playerCountPanel);
        }
 
        // 自動支店切り替え選択後GameSceneに移行
        public void OnClickAutoSwitch()
        {
            SceneController.Instance.cameraMode = CameraMode.AutoSwitch;
            SceneController.Instance.ChangeScene("GameScene");
        }
        // 俯瞰視点固定選択後GameSceneに移行
        public void OnClickOverhead()
        {
            SceneController.Instance.cameraMode = CameraMode.Overhead;
            SceneController.Instance.ChangeScene("GameScene");
        }
        // 視点選択からゲームモード選択に戻る
        public void OnClickBackToGameMode()
        {
            ShowPanel(modePanel);
        }

        // パネル切り替え共通処理
        private void ShowPanel(CanvasGroup panel)
        {
            // 全パネルを隠す
            titlePanel.alpha = 0;
            titlePanel.interactable = false;
            titlePanel.blocksRaycasts = false;

            playerCountPanel.alpha = 0;
            playerCountPanel.interactable = false;
            playerCountPanel.blocksRaycasts = false;

            modePanel.alpha = 0;
            modePanel.interactable = false;
            modePanel.blocksRaycasts = false;

            cameraModePanel.alpha = 0;
            cameraModePanel.interactable = false;
            cameraModePanel.blocksRaycasts = false;

            // 指定パネルだけ表示
            panel.alpha = 1;
            panel.interactable = true;
            panel.blocksRaycasts = true;
        }
    }
}

