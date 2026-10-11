// GamePlay/BGMManager.cs（新規）
using UnityEngine;

namespace Chess.GamePlay
{
    public class BGMManager : MonoBehaviour
    {
        public static BGMManager Instance;

        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip titleBGM;   // タイトル・リザルト共通
        [SerializeField] private AudioClip gameBGM;    // ゲーム中

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

        public void PlayTitleBGM()
        {
            if (titleBGM == null) return;  // ファイル未設定時は何もしない
            if (audioSource.clip == titleBGM && audioSource.isPlaying) return;
            audioSource.clip = titleBGM;
            audioSource.loop = true;
            audioSource.Play();
        }

        public void PlayGameBGM()
        {
            if (gameBGM == null) return;
            if (audioSource.clip == gameBGM && audioSource.isPlaying) return;
            audioSource.clip = gameBGM;
            audioSource.loop = true;
            audioSource.Play();
        }

        public void StopBGM()
        {
            audioSource.Stop();
        }
    }
}