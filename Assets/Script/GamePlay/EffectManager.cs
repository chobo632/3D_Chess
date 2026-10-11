using UnityEngine;

namespace Chess.GamePlay
{
    public class EffectManager : MonoBehaviour
    {
        public static EffectManager Instance;

        [SerializeField] private GameObject captureEffectPrefab;  // 駒を取った時のエフェクト

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

        // 駒を取った時に呼ぶ
        public void PlayCaptureEffect(Vector3 position)
        {
            if (captureEffectPrefab == null) return;  // 未設定時は何もしない
            var effect = Instantiate(captureEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 3f);  // 3秒後に自動削除
        }
    }
}