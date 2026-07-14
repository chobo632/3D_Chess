using System.Collections;
using UnityEngine;
using UnityEngine.UI;   
using TMPro;
using Chess.Scene;
using Chess.GamePlay;

namespace Chess.UI
{
    public class CointossPanel : MonoBehaviour
    {
        // コイントスパネル
        [SerializeField] private CanvasGroup cointossPanel;
        [SerializeField] private TextMeshProUGUI cointossResultText;
        [SerializeField] private GameObject headButton;
        [SerializeField] private GameObject tailButton;
        [SerializeField] private Button cointossButton;

        // Start is called before the first frame update
        void Start()
        {
            // 
            cointossButton.interactable = false;
            cointossResultText.gameObject.SetActive(false);
        }

        // 結果表示後、数秒でパネルを閉じる
        private IEnumerator CloseCointossPanel()
        {
            yield return new WaitForSeconds(2.0f);          // yield return 一時停止後後ろに書いた処理を実行する
            GameScene.Instance.EndCointoss();
        }

        private void Awake()
        {
            cointossButton.interactable = false;
            cointossResultText.gameObject.SetActive(false);
        }

        // コイン表選択ボタン
        public void OnClickHead()
        {
            Debug.Log("OnClickHead called");

            GameManager.Instance.SetPlayerChoice(true);
            cointossButton.interactable = true;
        }
        // コイン裏選択ボタン
        public void OnClickTail()
        {
            GameManager.Instance.SetPlayerChoice(false);
            cointossButton.interactable = true;
        }

        // コイントス開始
        public void OnClickCointoss()
        {
            GameManager.Instance.ExecuteCointoss();
            cointossResultText.gameObject.SetActive(true);
            // 表裏ボタンとコイントスボタンを非活性化
            headButton.SetActive(false);
            tailButton.SetActive(false);
            cointossButton.gameObject.SetActive(false);
            StartCoroutine(CloseCointossPanel());
        }

        // コイントスパネル表示
        public void ShowCointoss(bool show)
        {
            cointossPanel.alpha = show ? 1 : 0;
            cointossPanel.interactable = show;
            cointossPanel.blocksRaycasts = show;
        }
        // コイントス結果テキスト更新
        public void SetCointossResult(string text)
        {
            cointossResultText.text = text;
        }

    }
}
