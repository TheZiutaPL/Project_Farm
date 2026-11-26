using InteractionSystem;
using InventorySystem;
using Popups;
using Popups.Specific;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Requests
{
    public class RequestObject : MonoBehaviour, ITargetable, IInteractable, IPopable
    {
        public string TargetableName => $"Request #{RequestID}";
        public Action<bool> OnTargetAction { get; set; }

        public int RequestID { get; private set; }

        public string PopupName => TargetableName;

        public PopupType PopupType => PopupType.Request;

        public Vector3 Position => transform.position + Vector3.up * 1.75f;

        private PopupInstance popupInstance;

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
            popupInstance = PopupUI.SetPopup(this);
        }

        public void OffTarget()
        {
            popupInstance.ReleasePopup();
        }

        public Dictionary<string, object> GetPopupInfo()
        {
            InventoryItemSO[] items = RequestManager.Instance.GetRequestedItems(RequestID, out List<InventoryItemSO> completedItems);

            return new Dictionary<string, object>()
            {
                { PopupInfoKeywords.REQUEST_ITEM_ARRAY, items },
                { PopupInfoKeywords.REQUEST_ITEM_COMPLETED_LIST, completedItems },
            };
        }
    }
}