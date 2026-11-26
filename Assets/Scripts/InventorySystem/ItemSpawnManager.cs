using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace InventorySystem
{
    public class ItemSpawnManager : NetworkBehaviour
    {
        private static ItemSpawnManager instance;

        [SerializeField] private PickableItem pickablePrefab;

        [SerializeField] private float radiusPerLog = .15f;

        public override void OnNetworkSpawn()
        {
            if (instance == null)
                instance = this;
            else
                NetworkObject.Despawn();
        }

        public static void SpawnItems(Vector3 spawnPosition, params InventoryItemSO[] items)
        {
            if (instance == null)
            {
                Debug.LogError("You're trying to spawn item pickables, but there is no ItemSpawnManager instance!");
                return;
            }

            instance.SpawnItemsRpc(spawnPosition, items.Select(x => x.ItemID).ToArray());
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void SpawnItemsRpc(Vector3 spawnPosition, params int[] itemIDs)
        {
            if (itemIDs == null || itemIDs.Length == 0)
                return;

            float radius = Mathf.Log(itemIDs.Length, 2) * radiusPerLog;

            foreach (int itemID in itemIDs)
            {
                Vector2 randomPositionOffset = Random.insideUnitCircle * radius;
                float rot = Random.Range(0f, 360f);

                PickableItem pickable = Instantiate(pickablePrefab, spawnPosition + new Vector3(randomPositionOffset.x, 0, randomPositionOffset.y), Quaternion.Euler(0, rot, 0));
                pickable.NetworkObject.Spawn(true);
                pickable.SetItemRpc(itemID);
            }
        }
    }
}