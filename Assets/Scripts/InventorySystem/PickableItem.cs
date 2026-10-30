using Databases;
using InteractionSystem;
using Unity.Netcode;
using UnityEngine;

namespace InventorySystem
{
    public class PickableItem : BaseTargetable, IInteractable
    {
        [SerializeField] private InventoryItemSO item;
        public override string TargetableName => item.ItemName;

        public void Interact()
        {
            if (item != null)
            {
                ItemPickupRpc();
            }
            else
                NetworkObject.Despawn();
        }

        [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
        public void SetItemRpc(int itemID)
        {
            InventoryItemSO itemSO = GameDatabase.Instance.ItemDatabase.GetDataByID(itemID);

            if(itemSO == null)
            {
                NetworkObject.Despawn();
                return;
            }

            this.item = itemSO;
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void ItemPickupRpc(RpcParams rpcParams = default)
        {
            if (item != null && rpcParams.GetNetworkPlayerInstance(out NetworkPlayerInstance playerInstance))
            {
                // Player without an inventory shouldn't be able to pickup an item
                if (!playerInstance.TryGetComponent(out Inventory inventory))
                    return;

                // If couldn't add item to inventory
                if (!inventory.AddItem(item))
                    return;
            }

            NetworkObject.Despawn();
        }
    }
}