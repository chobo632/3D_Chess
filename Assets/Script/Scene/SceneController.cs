using UnityEngine;
using UnityEngine.SceneManagement;
using Chess.Core;

namespace Chess.Scene
{
    public class SceneController : MonoBehaviour
    {
        // インスタンス化
        public static SceneController Instance;
        // プレイ人数の選択表示
        public PlayerCount playerCount;
        // ゲームモード選択表示
        public GameMode selectMode;
        // 2人プレイ時視点選択
        public CameraMode cameraMode;
        // 勝敗表示
        public GameResult gameResult;

        // 
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // シーン切り替え
        public void ChangeScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}

