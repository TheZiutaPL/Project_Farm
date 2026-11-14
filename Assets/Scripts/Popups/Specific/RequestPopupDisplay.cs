using InventorySystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Popups.Specific
{
    public class RequestPopupDisplay : PopupDisplay
    {
        [SerializeField] private TextMeshProUGUI requestNameText;
        [SerializeField] private RequestItemDisplay[] itemDisplays = new RequestItemDisplay[0];

        protected override void SetPopupInfo(string popupName, Dictionary<string, object> popupInfo)
        {
            requestNameText.SetText(popupName);

            if (popupInfo.TryGetValue(PopupInfoKeywords.REQUEST_ITEM_ARRAY, out object itemListObject) && popupInfo.TryGetValue(PopupInfoKeywords.REQUEST_ITEM_COMPLETED_LIST, out object completedListObject))
                SetRequestItems(itemListObject as InventoryItemSO[], completedListObject as List<InventoryItemSO>);
            else
                ClearRequestItems();
        }

        public void SetRequestItems(InventoryItemSO[] requestedItems, List<InventoryItemSO> completedItems)
        {
            if (requestedItems == null) 
            {
                ClearRequestItems();
                return;
            }

            for (int i = 0; i < itemDisplays.Length; i++)
            {
                if(i >= requestedItems.Length)
                {
                    itemDisplays[i].ClearDisplay();
                    itemDisplays[i].SetCompleted(false);
                    continue;
                }

                itemDisplays[i].SetDisplay(requestedItems[i]);

                bool completed = completedItems != null && completedItems.Contains(requestedItems[i]);
                itemDisplays[i].SetCompleted(completed);
                if (completed)
                    completedItems.Remove(requestedItems[i]);
            }
        }

        public void ClearRequestItems()
        {
            for (int i = 0; i < itemDisplays.Length; i++)
            {
                itemDisplays[i].ClearDisplay();
                itemDisplays[i].SetCompleted(false);
            }
        }
    }
}