using System.Collections.Generic;
using UnityEngine;

namespace Chess.Core
{
    public class BoardModel : MonoBehaviour
    {
        [SerializeField] GameObject moveMarkerPrefab;

        // 各駒のプレハブ
        [SerializeField] private PieceView pawnPrefab;
        [SerializeField] private PieceView rookPrefab;
        [SerializeField] private PieceView knightPrefab;
        [SerializeField] private PieceView bishopPrefab;
        [SerializeField] private PieceView queenPrefab;
        [SerializeField] private PieceView kingPrefab;
        // マス座標、マスサイズ
        [SerializeField] private float cellSize = 0.239f;
        [SerializeField] private float boardReferencepos = 3.5f;

        private const int BoardSize_X = 8;
        private const int BoardSize_Y = 8;
        
        private readonly List<GameObject> moveMarkers = new();
        // PieceデータとPieceControllerの対応を管理
        private readonly Dictionary<PieceModel, PieceView> controllers = new();

        public CellModel Model { get; } = new CellModel();


        public struct PieceData
        {
            public PieceColor pieceColor;
            public PieceType  pieceType;
        }

        public void RegisterController(PieceModel piece, PieceView pieceController)
        {
            controllers[piece] = pieceController;
        }

        public PieceView GetController(PieceModel piece)
        {
            controllers.TryGetValue(piece, out var controller);
            return controller;
        }

        public void RemoveController(PieceModel piece)
        {
            controllers.Remove(piece);
        }

        // Start is called before the first frame update
        private void Start()
        {
            var layout = BuildInitialLayout();

            // 駒の生成
            for (int x = 0; x < BoardSize_X; x++)
            {
                for (int y = 0; y < BoardSize_Y; y++)
                {
                    if (layout[x, y].pieceType != PieceType.None)
                    {
                        SpawnPiece(layout[x, y].pieceType, layout[x, y].pieceColor, new Vector2Int(x, y));
                    }
                }
            }
        }

        private PieceData[,] BuildInitialLayout()
        {
            var layout = new PieceData[BoardSize_X, BoardSize_Y];

            // 盤上後方の駒整列順
            PieceType[] backRow =
            { PieceType.Rook,PieceType.Knight,PieceType.Bishop,PieceType.King,
              PieceType.Queen,PieceType.Bishop,PieceType.Knight,PieceType.Rook };

            // 駒の配置
            for (int x = 0; x < BoardSize_X; x++)
            {
                layout[x, 6] = new PieceData
                { pieceColor = PieceColor.White, pieceType = PieceType.Pawn };

                layout[x, 7] = new PieceData
                { pieceColor = PieceColor.White, pieceType = backRow[x] };

                layout[x, 1] = new PieceData
                { pieceColor = PieceColor.Black, pieceType = PieceType.Pawn };

                layout[x, 0] = new PieceData
                { pieceColor = PieceColor.Black, pieceType = backRow[x] };

            }

            return layout;
        }

        // 盤座標取得
        public Vector3 GetWorldPosition(Vector2Int pos)
        {
            return transform.position + new Vector3((pos.x - boardReferencepos) * cellSize, 0.15f, (pos.y - boardReferencepos) * cellSize);
        }

        public Vector2Int GetBoardPosition(Vector3 worldPos)
        {
            float x = Mathf.Round((worldPos.x - transform.position.x + cellSize / 2) / cellSize) + boardReferencepos;
            float y = Mathf.Round((worldPos.z - transform.position.z + cellSize / 2) / cellSize) + boardReferencepos;

            return new Vector2Int((int)x, (int)y);
        }

        // 生成関数
        public void SpawnPiece(PieceType type, PieceColor color, Vector2Int pos)
        {
            var prefab = GetPrefab(type);
            // カメラの視点切り替え調整
            var rotation = color == PieceColor.White ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
            var controller = Instantiate(prefab, GetWorldPosition(pos), rotation);

            // Pieceデータを生成してControllerに渡す
            var piece = new PieceModel(type, color);
            controller.Initialize(piece);
            
            Model.Place(piece, pos);
            RegisterController(piece, controller);
        }

        // 
        private PieceView GetPrefab(PieceType type)
        {
            PieceView prefab = null;

            switch (type)
            {
                case PieceType.Pawn:
                    prefab = pawnPrefab; break;
                case PieceType.Rook:
                    prefab = rookPrefab; break;
                case PieceType.Knight:
                    prefab = knightPrefab; break;
                case PieceType.Bishop:
                    prefab = bishopPrefab; break;
                case PieceType.Queen:
                    prefab = queenPrefab; break;
                case PieceType.King:
                    prefab = kingPrefab; break;
            }

            return prefab;
        }

        // 駒の移動実行
        public void ExecuteMove(PieceModel piece, Vector2Int pos)
        {
            var captured = Model.MovePiece(piece, pos);

            // 取った駒削除
            if (captured != null)
            {
                DestroyPieceController(captured);
            }

            // 見た目更新
            var controller  = GetController(piece);
            controller.MoveTo(GetWorldPosition(pos));
            controller.IsMoved = true;
        }

        // 駒の削除
        public void RemovePiece(Vector2Int pos)
        {
            var target = Model.GetPiece(pos);

            if (target != null)
            {
                DestroyPieceController(target);
                Model.ClearCell(pos);
            }
        }

        public void PromotePiece(PieceModel piece, PieceType type)
        {
            var pos = Model.GetPosition(piece);
            var color = piece.PieceColor;
            RemovePiece(pos);
            SpawnPiece (type, color, pos);
        }

        // 移動可能マスの色変更
        public void ShowMoves(List<Vector2Int> moves)
        {
            foreach (var pos in moves)
            {
                var marker = Instantiate(moveMarkerPrefab, GetWorldPosition(pos), Quaternion.identity);
                moveMarkers.Add(marker);
            }
        }

        // マーカーの削除
        public void HideMoves()
        {
            foreach (var marker in moveMarkers)
            {
                Destroy(marker);
            }

            moveMarkers.Clear();
        }

        // 
        private void DestroyPieceController(PieceModel piece)
        {
            var controller = GetController(piece);

            if (controller != null)
            {
                Destroy(controller.gameObject);
                RemoveController(piece);
            }
        }

        // Modelの読み取りを中継
        public PieceModel GetPiece(Vector2Int pos)
        {
            return Model.GetPiece(pos);
        }
        public List<PieceModel> GetPieces(PieceColor color)
        {
            return Model.GetPieces(color);
        }
        public PieceModel GetKing(PieceColor color)
        {
            return Model.GetKing(color);
        }
    }
}
