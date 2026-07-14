using UnityEngine;
using Chess.GamePlay;
using Chess.UI;
using Chess.Core;

namespace Chess.Scene
{
    public class GameScene : MonoBehaviour
    {
        public static GameScene Instance;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // イベントに登録
            GameManager.Instance.TurnChanged += GameUIManager.Instance.UpdateTurnText;
            GameManager.Instance.CheckChanged += GameUIManager.Instance.ShowCheckText;
            GameManager.Instance.PromotionRequested += GameUIManager.Instance.ShowPromotion;
            GameManager.Instance.GameEnd += GameEnd;
            GameManager.Instance.CointossResultReady += GameUIManager.Instance.SetCointossResult;
            GameManager.Instance.RandomModeStarted += OnRandomModeStarted;
            GameManager.Instance.LotteryPiecesUpdated += GameUIManager.Instance.ShowLotteryPieces;
            GameManager.Instance.RerollCountUpdated += GameUIManager.Instance.UpdateRerollText;

            // GameManagerにModeをセット
            GameManager.Instance.SetMode(SceneController.Instance.selectMode);

            // 最初はポーズパネルを非表示
            GameUIManager.Instance.ShowPause(false);

            // コイントス開始
            StartCointoss();
        }

        // RandomModeパネル表示
        private void OnRandomModeStarted()
        {
            GameUIManager.Instance.ShowRandomMode(true);
        }

        // コイントス画面を表示させる
        private void StartCointoss()
        {
            GameManager.Instance.IsCointoss = true;
            GameUIManager.Instance.ShowCointoss(true);
        }
        // コイントス画面を非表示にする
        public void EndCointoss()
        {
            GameManager.Instance.IsCointoss = false;
            GameUIManager.Instance.ShowCointoss(false);

            // ターンテキスト初期表示
            GameUIManager.Instance.UpdateTurnText(GameManager.Instance.turnCount);

            // RandomMode時は初回抽選
            if (GameManager.Instance.IsRandomModeActive())
            {
                GameManager.Instance.StartLottery();
            }
        }

        private void Update()
        {
            // EscかPでポーズ切り替え
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                GameManager.Instance.IsPaused = !GameManager.Instance.IsPaused;
                GameUIManager.Instance.ShowPause(GameManager.Instance.IsPaused);
            }
        }

        // Resetボタン
        public void OnClickReset()
        {
            SceneController.Instance.ChangeScene("GameScene");
        }

        // Titleボタン
        public void OnClickTitle()
        {
            SceneController.Instance.ChangeScene("TitleScene");
        }

        // 
        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TurnChanged -= GameUIManager.Instance.UpdateTurnText;
                GameManager.Instance.CheckChanged -= GameUIManager.Instance.ShowCheckText;
                GameManager.Instance.PromotionRequested -= GameUIManager.Instance.ShowPromotion;
                GameManager.Instance.GameEnd -= GameEnd;
                GameManager.Instance.CointossResultReady -= GameUIManager.Instance.SetCointossResult;
                GameManager.Instance.RandomModeStarted -= OnRandomModeStarted;
                GameManager.Instance.LotteryPiecesUpdated -= GameUIManager.Instance.ShowLotteryPieces;
                GameManager.Instance.RerollCountUpdated -= GameUIManager.Instance.UpdateRerollText;
            }
        }

        // 
        private void GameEnd(GameResult result)
        {
            // ResultSceneに移行
            SceneController.Instance.gameResult = result;
            SceneController.Instance.ChangeScene("ResultScene");
        }
    }
}
