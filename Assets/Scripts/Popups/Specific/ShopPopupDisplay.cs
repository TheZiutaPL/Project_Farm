using InventorySystem;
using InventorySystem.UI;
using Shop;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Popups.Specific
{
    public class ShopPopupDisplay : PopupDisplay
    {
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI buyText;
        [SerializeField] private ItemDisplay itemDisplay;
        [SerializeField] private TextMeshProUGUI priceText;

        protected override void SetPopupInfo(string popupName, Dictionary<string, object> popupInfo)
        {
            headerText.SetText(popupName);

            if(popupInfo.TryGetValue(PopupInfoKeywords.SHOP_ITEM, out object shopItemObject) && shopItemObject is InventoryItemSO shopItem)
            {
                if (NetworkPlayerInstance.OwnerInstance.Inventory.HasEmptySlot())
                {
                    bool canBuy = MoneyManager.GetMoney() >= shopItem.BuyPrice;
                    buyText.SetText(canBuy ? "[E] to buy" : "Too pricey");
                }
                else
                    buyText.SetText("No space");

                itemDisplay.SetDisplay(shopItem);
                priceText.SetText(shopItem.BuyPrice.ToString());
            }
        }
    }
}