using InteractionSystem;
using InventorySystem;
using System;
using UnityEngine;

namespace Requests
{
    public class RequestObject : MonoBehaviour, ITargetable, IInteractable
    {
        public string TargetableName => $"Request #{RequestID}";
        public Action<bool> OnTargetAction { get; set; }

        public int RequestID { get; private set; }

        public void SetRequestID(int requestID)
        {
            RequestID = requestID;
        }

        public void Interact()
        {
            RequestManager.Instance.SubmitItemsFromPlayerRpc(RequestID);
        }

        public void OnTarget()
        {
            InventoryItemSO[] items = RequestManager.Instance.GetRequestItems(RequestID, true);
            string str = TargetableName + "\n";
            for (int i = 0; i < items.Length; i++)
            {
                str += $"{items[i].ItemName}, ";
            }

            Debug.Log(str);
        }

        public void OffTarget()
        {

        }
    }
}