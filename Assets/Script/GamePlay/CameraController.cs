using UnityEngine;
using Chess.Core;

namespace Chess.GamePlay
{
    public class CameraController : MonoBehaviour
    {
        // 白・黒それぞれの視点設定
        [SerializeField] private Vector3 whitePosition = new Vector3(0f, 1.2f, 1.0f);
        [SerializeField] private Vector3 whiteRotation = new Vector3(50f, 180f, 0f);
        [SerializeField] private Vector3 blackPosition = new Vector3(0f, 1.2f, -1.0f);
        [SerializeField] private Vector3 blackRotation = new Vector3(50f, 0f, 0f);
        // 俯瞰視点設定
        [SerializeField] private Vector3 overheadPosition = new Vector3(0f, 2.5f, 0f);
        [SerializeField] private Vector3 overheadRotation = new Vector3(90f, 0f, 0f);

        private bool isOverhead = false;

        // Start is called before the first frame update
        void Start()
        {
            SetWhiteView();
        }

        // Update is called once per frame
        void Update()
        {
            // Fキーで俯瞰トグル
            if (Input.GetKeyDown(KeyCode.F))
            {
                isOverhead = !isOverhead;

                if (isOverhead)
                {
                    ApplyView(overheadPosition, overheadRotation);
                }
                else
                {
                    // 俯瞰解除時は現在のターン視点に戻す
                    SetViewByTurn(GameManager.Instance.currentTurn);
                }
            }
        }

        // ターン切り替え時にGameManagerから呼ぶ
        public void SetViewByTurn(PieceColor turn)
        {
            // 俯瞰中はターン切り替えでカメラを動かさない
            if (isOverhead)
            {
                return;
            }

            if (turn == PieceColor.White)
            {
                SetWhiteView();
            }
            else
            {
                SetBlackView();
            }
        }

        private void SetWhiteView()
        {
            ApplyView(whitePosition, whiteRotation);
        }

        private void SetBlackView()
        {
            ApplyView(blackPosition, blackRotation);
        }

        private void ApplyView(Vector3 position, Vector3 rotation)
        {
            transform.position    = position;
            transform.eulerAngles = rotation;
        }
    }
}
