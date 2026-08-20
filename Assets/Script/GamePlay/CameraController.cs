using UnityEngine;
using Chess.Core;
using Chess.Scene;

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
        [SerializeField] private Vector3 overheadRotationWhite = new Vector3(90f, 180f, 0f); // 白が手前
        [SerializeField] private Vector3 overheadRotationBlack = new Vector3(90f, 0f, 0f);   // 黒が手前

        private bool isOverhead = false;
        // プレイヤーの色を保持
        private PieceColor playerColor = PieceColor.White;

        // Start is called before the first frame update
        void Start()
        {
            var playerCount = SceneController.Instance.playerCount;
            var cameraMode = SceneController.Instance.cameraMode;

            if (playerCount == PlayerCount.DuoPlay && cameraMode == CameraMode.Overhead)
            {
                // 最初から俯瞰にして変更不可
                ApplyOverheadView();
                isOverhead = true;
                return;
            }

            // 白視点で開始(Soloはプレイヤーの色で上書き)
            SetWhiteView();
        }

        // Update is called once per frame
        void Update()
        {
            // DuoPlay俯瞰固定の場合はFキー無効
            if (SceneController.Instance.playerCount == PlayerCount.DuoPlay && SceneController.Instance.cameraMode == CameraMode.Overhead)
            {
                return;
            }

            // Fキーで俯瞰トグル
            if (Input.GetKeyDown(KeyCode.F))
            {
                isOverhead = !isOverhead;

                if (isOverhead)
                {
                    ApplyOverheadView();
                }
                else
                {
                    RestoreView();
                }
            }
        }

        // コイントス後にプレイヤーの色を設定して視点固定
        public void SetPlayerColor(PieceColor color)
        {
            playerColor = color;
            if (!isOverhead)
            {
                ApplyColorView(color);
            }
        }

        // 
        private void ApplyOverheadView()
        {
            if (SceneController.Instance.playerCount == PlayerCount.SoloPlay)
            {
                // プレイヤーの色に応じて俯瞰の向きを変える
                var rotation = playerColor == PieceColor.White ? overheadRotationWhite : overheadRotationBlack;
                ApplyView(overheadPosition, rotation);
            }
            else
            {
                ApplyView(overheadPosition, overheadRotationWhite);
            }
        }

        // ターン切り替え時にGameManagerから呼ぶ
        public void SetViewByTurn(PieceColor turn)
        {   
            // 俯瞰中はターン切り替えでカメラを動かさない

            if (SceneController.Instance.playerCount == PlayerCount.DuoPlay && SceneController.Instance.cameraMode == CameraMode.Overhead)
            {
                return;
            }

            if (SceneController.Instance.playerCount == PlayerCount.SoloPlay)
            {
                return;
            }
  
            if (isOverhead)
            {
                return;
            }

            ApplyColorView(turn);
        }

        // 俯瞰解除時に適切な視点に戻す
        private void RestoreView()
        {
            if (SceneController.Instance.playerCount == PlayerCount.SoloPlay)
            {
                // SoloPlay：プレイヤーの色の視点に戻す
                ApplyColorView(playerColor);
            }
            else
            {
                // DuoPlay自動切り替え：現在のターンの視点に戻す
                ApplyColorView(GameManager.Instance.currentTurn);
            }
        }

        private void ApplyColorView(PieceColor color)
        {
            if (color == PieceColor.White)
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
