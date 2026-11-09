using Databases;
using InventorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Requests
{
    public class RequestManager : NetworkBehaviour
    {
        public static RequestManager Instance { get; private set; }

        [SerializeField] private int maxActiveRequests = 3;
        [SerializeField] private float requestWaitTime = 12f;
        private float requestTimer;

        private NetworkList<RequestData> activeRequests = new NetworkList<RequestData>();

        [Header("Request Generation")]
        [SerializeField] private Vector2Int minMaxRequestedItems = new Vector2Int(1, 4);
        [SerializeField] private InventoryItemSO[] requestableItems = new InventoryItemSO[0];
        private int requestIndex;

        public HashSet<int> GetActiveRequestIDs()
        {
            HashSet<int> requestIDs = new HashSet<int>();

            for (int i = 0; i < activeRequests.Count; i++)
            {
                if (!requestIDs.Contains(activeRequests[i].RequestID))
                    requestIDs.Add(activeRequests[i].RequestID);
            }
            
            return requestIDs;
        }

        public static Action OnRequestsUpdated;

        public override void OnNetworkSpawn()
        {
            activeRequests.OnListChanged += RequestsUpdated;

            if (Instance == null)
                Instance = this;
        }

        private void RequestsUpdated(NetworkListEvent<RequestData> changeEvent)
        {
            if (IsServer)
                requestTimer = requestWaitTime;

            OnRequestsUpdated?.Invoke();
        }

        private void Update()
        {
            if (!IsServer || Instance != this)
                return;

            HandleRequestUpdate();
        }

        private void HandleRequestUpdate()
        {
            requestTimer -= Time.deltaTime;

            if(requestTimer <= 0)
            {
                requestTimer = requestWaitTime;

                // If requests are full
                if (GetActiveRequestIDs().Count >= maxActiveRequests)
                    return;

                CreateNewRequest();
            }
        }

        private void CreateNewRequest()
        {
            if (!IsServer)
                return;

            requestIndex++;
            int requestedItems = Random.Range(minMaxRequestedItems.x, minMaxRequestedItems.y);
            for (int i = 0; i < requestedItems; i++)
            {
                activeRequests.Add(new RequestData()
                {
                    RequestID = requestIndex,
                    RequestedItemID = requestableItems[Random.Range(0, requestableItems.Length)].ItemID,
                });
            }

            Debug.Log($"Added new Request #{requestIndex}!");
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void SubmitItemsFromPlayerRpc(int targetRequestID, RpcParams rpcParams = default)
        {
            if (!rpcParams.GetNetworkPlayerInstance(out NetworkPlayerInstance networkPlayerInstance))
                return;

            if (networkPlayerInstance.Inventory == null)
                return;

            List<InventoryItemSO> playerItems = networkPlayerInstance.Inventory.GetItems();

            // If is targeting a request
            if(targetRequestID >= 0)
            {
                int[] indexes = GetRequestDataIndexes(targetRequestID);
                foreach (var index in indexes)
                {
                    if (playerItems.Count <= 0)
                        break;

                    // Check if index is valid
                    if (index < 0 || index >= activeRequests.Count)
                        continue;

                    RequestData requestData = activeRequests[index];

                    // Check if is not already completed
                    if (requestData.IsCompleted)
                        continue;

                    // Check if any of player items is requested item
                    InventoryItemSO itemMatch = playerItems.Find(x => x.ItemID == requestData.RequestedItemID);
                    if (itemMatch == null)
                        continue;

                    // Remove item from player
                    playerItems.Remove(itemMatch);
                    networkPlayerInstance.Inventory.RemoveItem(itemMatch);

                    // Set requestData completed
                    requestData.IsCompleted = true;
                    activeRequests.Set(index, requestData);
                }
            }
            // Fill up any active request
            else
            {
                for (int i = 0; i < activeRequests.Count && playerItems.Count > 0; i++)
                {
                    RequestData requestData = activeRequests[i];

                    // Check if is not already completed
                    if (requestData.IsCompleted)
                        continue;

                    // Check if any of player items is requested item
                    InventoryItemSO itemMatch = playerItems.Find(x => x.ItemID == requestData.RequestedItemID);
                    if (itemMatch == null)
                        continue;

                    // Remove item from player
                    playerItems.Remove(itemMatch);
                    networkPlayerInstance.Inventory.RemoveItem(itemMatch);

                    // Set requestData completed
                    requestData.IsCompleted = true;
                    activeRequests.Set(i, requestData);
                }
            }

            // Check if there are completed orders
            List<int> completedRequests = CheckForCompletedRequests();
            for (int i = activeRequests.Count - 1; i >= 0; i--)
            {
                if (!completedRequests.Contains(activeRequests[i].RequestID))
                    continue;

                Debug.Log($"Completed Request #{activeRequests[i].RequestID}");
                activeRequests.RemoveAt(i);
            }
        }

        private List<int> CheckForCompletedRequests()
        {
            if (!IsServer)
                return new List<int>();

            Dictionary<int, bool> requests = new Dictionary<int, bool>();

            foreach (var data in activeRequests)
            {
                if (!requests.ContainsKey(data.RequestID))
                {
                    requests.Add(data.RequestID, data.IsCompleted);
                    continue;
                }

                if (data.IsCompleted)
                    continue;

                requests[data.RequestID] = false;
            }

            return requests.Where(x => x.Value).Select(x => x.Key).ToList();
        }

        private int[] GetRequestDataIndexes(int targetRequestID)
        {
            if (targetRequestID < 0)
                return new int[0];

            List<int> dataIndexes = new List<int>();

            for (int i = 0; i < activeRequests.Count; i++)
            {
                if (targetRequestID != activeRequests[i].RequestID)
                    continue;

                dataIndexes.Add(i);
            }

            return dataIndexes.ToArray();
        }

        public InventoryItemSO[] GetRequestItems(int targetRequestID, bool skipCompleted)
        {
            if(targetRequestID < 0)
                return new InventoryItemSO[0];

            List<InventoryItemSO> items = new List<InventoryItemSO>();

            for (int i = 0; i < activeRequests.Count; i++)
            {
                if (targetRequestID != activeRequests[i].RequestID || (skipCompleted && activeRequests[i].IsCompleted))
                    continue;

                items.Add(GameDatabase.Instance.ItemDatabase.GetDataByID(activeRequests[i].RequestedItemID));
            }

            return items.ToArray();
        }
    }
}