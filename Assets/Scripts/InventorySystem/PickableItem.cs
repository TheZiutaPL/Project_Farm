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
            // TODO pickup
            if (item != null)
            {
                ItemPickupServerRpc();
            }
            else
                NetworkObject.Despawn();
        }

        [ServerRpc(InvokePermission = RpcInvokePermission.Everyone)]
        public void ItemPickupServerRpc(ServerRpcParams rpcParams = default)
        {
            if (item != null && NetworkManager.Singleton.ConnectedClients.TryGetValue(rpcParams.Receive.SenderClientId, out NetworkClient networkClient))
            {
                // Player without an inventory shouldn't be able to pickup an item
                if (!networkClient.PlayerObject.TryGetComponent(out Inventory inventory))
                    return;

                // If couldn't add item to inventory
                if (!inventory.AddItem(item))
                    return;
            }

            NetworkObject.Despawn(gameObject);
        }
    }
}