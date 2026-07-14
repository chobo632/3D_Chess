using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Chess.Core;

namespace Chess.Scene
{
    public class ResultScene : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI resultText;
        [SerializeField] Image resultImageWhite;
        [SerializeField] Image resultImageBlack;
        [SerializeField] Image resultImageDraw;

        // TitleSceneに移行
        public void OnClickTitle()
        {
            SceneController.Instance.ChangeScene("TitleScene");
        }
        // Exitボタン
        public void OnClickExit()
        {
            Application.Quit();
        }


        // 勝敗判定テキスト表示
        private void Start()
        {
            // 全画像を非表示
            resultImageWhite.gameObject.SetActive(false);
            resultImageBlack.gameObject.SetActive(false);
            resultImageDraw.gameObject.SetActive(false);

            switch (SceneController.Instance.gameResult)
            {
                case GameResult.WhiteWin:
                    resultText.text = "White Win!";
                    resultImageWhite.gameObject.SetActive(true);
                    break;
                case GameResult.BlackWin:
                    resultText.text = "Black Win!";
                    resultImageBlack.gameObject.SetActive(true);
                    break;
                case GameResult.Draw:
                    resultText.text = "Draw...";
                    resultImageDraw.gameObject.SetActive(true);
                    break;
            }
        }
    }
}
