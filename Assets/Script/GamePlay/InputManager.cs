using System.Collections.Generic;
using UnityEngine;
using Chess.Core;

namespace Chess.GamePlay
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] BoardModel board;
        [SerializeField] private LayerMask boardLayerMask;

        private PieceModel selectPiece;
        private PieceView selectPieceController;
        private Color originalColor;
        private List<Vector2Int> currentLegalMoves = new();

        // Update is called once per frame
        void Update()
        {
            // ポーズ中は入力を受け付けない
            var gm = GameManager.Instance;

            if (gm.IsCointoss || gm.IsPaused || gm.IsPromotion)
            {
                return;
            }

            // 駒の選択
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, boardLayerMask))
                {
                    var boardPos = board.GetBoardPosition(hit.point);
                    // 駒が選択されたか
                    var piece = board.GetPiece(boardPos);

                    // 駒が選択されている
                    if (selectPiece == null)
                    {
                        TrySelectPiece(piece, boardPos);
                    }
                    // 駒が選択されていない
                    else
                    {
                        TryMoveOrReselect(piece, boardPos);
                    }
                }
            }
            // 選択解除
            else if (Input.GetMouseButtonDown(1))
            {
                DeselectPiece();
            }
        }

        // 駒を選択する
        private void TrySelectPiece(PieceModel piece, Vector2Int boardPos)
        {
            if (piece == null)
            {
                return;
            }
            // 自分のターンじゃないなら選択不可
            if (piece.PieceColor != GameManager.Instance.currentTurn)
            {
                return;
            }

            // AIの駒は選択不可
            if (GameManager.Instance.IsAIPiece(piece))
            {
                return;
            }

            // 抽選された駒のみ選択可能
            if (!GameManager.Instance.IsLotteryPiece(piece))
            {
                return;
            }

            SelectPiece(piece);
        }

        // 移動するか別の駒を選択する
        private void TryMoveOrReselect(PieceModel piece, Vector2Int boardPos)
        {
            // 自分の駒を選択し直す
            if (piece != null && piece.PieceColor == GameManager.Instance.currentTurn)
            {
                if (!GameManager.Instance.IsLotteryPiece(piece))
                {
                    return;
                }

                DeselectPiece();
                SelectPiece(piece);

                return;
            }

            board.HideMoves();

            if (currentLegalMoves.Contains(boardPos))
            {
                RestorePieceColor(selectPiece, selectPieceController);
                var movingPiece = selectPiece;
                selectPiece = null;
                selectPieceController = null;
                currentLegalMoves.Clear();

                GameManager.Instance.MoveRequest(movingPiece, boardPos);

                return;
            }
            else
            {
                // 移動不可なら元の位置に戻す
                selectPieceController.MoveTo(board.GetWorldPosition(board.Model.GetPosition(selectPiece)));
            }

            RestorePieceColor(selectPiece, selectPieceController);
            selectPiece = null;
            selectPieceController = null;
            currentLegalMoves.Clear();
        }

        // 駒を選択状態にする
        private void SelectPiece(PieceModel piece)
        {
            board.HideMoves();
            selectPiece = piece;
            selectPieceController = board.GetController(piece);

            // 移動可能なリストを取得
            currentLegalMoves = GameManager.Instance.GetLegalMoves(selectPiece);
            // 移動可能マス表示
            board.ShowMoves(currentLegalMoves);

            originalColor = selectPieceController.GetCurrentColor();
            selectPieceController.Lift(board.GetWorldPosition(board.Model.GetPosition(selectPiece)), 0.3f);
            selectPieceController.SetHighlightColor(new Color(1.0f, 0.5f, 0.5f, 1.0f));
        }

        private void RestorePieceColor(PieceModel piece, PieceView controller)
        {
            if (GameManager.Instance.IsRandomModeActive() && GameManager.Instance.IsLotteryPiece(piece))
            {
                controller.SetHighlightColor(new Color(0.5f, 0.5f, 1.0f, 1.0f));
            }
            else
            {
                controller.SetColor(piece.PieceColor);
            }
        }

        // 選択解除
        private void DeselectPiece()
        {
            if (selectPiece != null)
            {
                selectPieceController.MoveTo(board.GetWorldPosition(board.Model.GetPosition(selectPiece)));

                // 抽選された駒なら抽選色に戻す、そうでなければ元の色に戻す
                if (GameManager.Instance.IsLotteryPiece(selectPiece))
                {
                    selectPieceController.SetHighlightColor(new Color(0.5f, 0.5f, 1.0f, 1.0f));
                    RestorePieceColor(selectPiece, selectPieceController);
                }
            }

            board.HideMoves();
            selectPiece = null;
            selectPieceController= null;
            currentLegalMoves.Clear();
        }
    }
}
