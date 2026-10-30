using InventorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace RuntimeGrid
{
    public class RuntimeGridInstance : NetworkBehaviour
    {
        public static RuntimeGridInstance Instance;

        [SerializeField] private Grid grid;
        [SerializeField] private Vector2Int[] eligibleCells = new Vector2Int[0];

        private Dictionary<Vector2Int, RuntimeCell> gridCells = new Dictionary<Vector2Int, RuntimeCell>();

        private Plane gridPlane;
        public Plane GetGridPlane() => gridPlane;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            gridPlane = new Plane(Vector3.up, grid.transform.position.y);
        }

        private void Start()
        {
            gridCells = eligibleCells.ToDictionary(x => x, x => new RuntimeCell(x));
        }

        public Vector2Int WorldToCellPosition(Vector3 worldPosition) => (Vector2Int)grid.WorldToCell(worldPosition + grid.cellSize / 2);
        public Vector3 CellToWorldPosition(Vector2Int cellPosition) => grid.CellToWorld((Vector3Int)cellPosition);

        public RuntimeCell GetRuntimeCell(Vector3 worldPosition)
        {
            if (!TryGetRuntimeCell(worldPosition, out RuntimeCell cell))
                return null;

            return cell;
        }
        public RuntimeCell GetRuntimeCell(Vector2Int cellPosition)
        {
            if(!TryGetRuntimeCell(cellPosition, out RuntimeCell cell))
                return null;

            return cell;
        }

        public bool TryGetRuntimeCell(Vector3 worldPosition, out RuntimeCell cell)
        {
            Vector2Int cellPosition = WorldToCellPosition(worldPosition);
            return TryGetRuntimeCell(cellPosition, out cell);
        }
        public bool TryGetRuntimeCell(Vector2Int cellPosition, out RuntimeCell cell)
        {
            cell = null;

            if (!gridCells.ContainsKey(cellPosition))
                return false;

            cell = gridCells[cellPosition];
            return true;
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void UseItemOnGridRpc(Vector2Int cellPosition, int itemID, RpcParams rpcParams = default)
        {
            if (!rpcParams.GetNetworkPlayerInstance(out NetworkPlayerInstance networkPlayerInstance))
                return;

            InventoryItemSO item = ItemDatabase.Instance.GetItemByID(itemID);
            if (item == null)
            {
                Debug.LogWarning("Trying to use invalid item!");
                return;
            }

            if (!TryGetRuntimeCell(cellPosition, out RuntimeCell cell))
                return;

            item.GetItemUseAction()?.Invoke(networkPlayerInstance, cell);
        }

        private void OnDrawGizmos()
        {
            if (grid == null)
                return;

            for (int i = 0; i < eligibleCells.Length; i++)
                DrawGridCell(eligibleCells[i]);

            void DrawGridCell(Vector2Int position)
            {
                Vector3 worldPosition = CellToWorldPosition(position);

                Vector3 v1, v2, v3, v4;
                Vector3 halfCellSize = new Vector3(grid.cellSize.x, 0, grid.cellSize.y) / 2;
                Vector3 inventedAxisHalfCellSize = new Vector3(grid.cellSize.x, 0, -grid.cellSize.y) / 2;

                v1 = worldPosition - halfCellSize;
                v2 = worldPosition - inventedAxisHalfCellSize;
                v3 = worldPosition + halfCellSize;
                v4 = worldPosition + inventedAxisHalfCellSize;

                Gizmos.DrawLine(v1, v2);
                Gizmos.DrawLine(v2, v3);
                Gizmos.DrawLine(v3, v4);
                Gizmos.DrawLine(v4, v1);
                Gizmos.DrawWireSphere(worldPosition, .15f);
            }
        }
    }
}