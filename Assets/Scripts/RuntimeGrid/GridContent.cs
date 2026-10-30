using Unity.Netcode;
using UnityEngine;

namespace RuntimeGrid
{
    public class GridContent : NetworkBehaviour
    {
        [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
        public void BindToGridRpc(Vector2Int cellPosition)
        {
            RuntimeCell cell = RuntimeGridInstance.Instance.GetRuntimeCell(cellPosition);
            if (cell == null)
            {
                NetworkObject.Despawn();
                return;
            }

            cell.SetCellObject(this);
        }
    }
}