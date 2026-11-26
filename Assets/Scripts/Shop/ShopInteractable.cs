using InteractionSystem;
using InventorySystem;
using Popups;
using Popups.Specific;
using StatTracking;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Shop
{
    public class ShopInteractable : NetworkBehaviour, IInteractable, IPopable
    {
        [SerializeField] private InventoryItemSO soldItem;

        public string TargetableName => soldItem != null ? soldItem.ItemName : "Invalid Item";
        public Action<bool> OnTargetAction { get; set; }

        public string PopupName => TargetableName;

        public PopupType PopupType => PopupType.Shop;

        public Vector3 Position => transform.position + Vector3.up;

        private PopupInstance popupInstance;

        public void Interact()
        {
            if (soldItem == null)
                return;

            BuyItemRpc();
        }

        public void OnTarget()
        {
            popupInstance = PopupUI.SetPopup(this);
        }

        public void OffTarget()
        {
            popupInstance.ReleasePopup();
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void BuyItemRpc(RpcParams rpcParams = default)
        {
            if (!rpcParams.GetNetworkPlayerInstance(out NetworkPlayerInstance networkPlayerInstance))
                return;

            if (!networkPlayerInstance.Inventory.HasEmptySlot())
                return;

            if (!MoneyManager.Instance.HasMoney(soldItem.BuyPrice))
                return;

            MoneyManager.Instance.RemoveMoney(soldItem.BuyPrice);
            networkPlayerInstance.Inventory.AddItem(soldItem);

            StatTrackingManager.Instance.IncrementStat(GameStats.ITEMS_BOUGHT);

            popupInstance.RefreshPopup();
        }

        public Dictionary<string, object> GetPopupInfo() => new Dictionary<string, object>()
        {
            { PopupInfoKeywords.SHOP_ITEM, soldItem }
        };
    }
}