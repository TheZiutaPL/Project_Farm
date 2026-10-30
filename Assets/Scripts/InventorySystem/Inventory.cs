using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InventorySystem
{
    public class Inventory : NetworkBehaviour
    {
        [field: SerializeField] public int InventorySize { get; private set; } = 5;
        private NetworkList<int> inventory = new NetworkList<int>
            (
            new int[0],
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server
            );

        public InventoryItemSO GetItemAtSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= InventorySize)
            {
                Debug.LogError("You're trying to access unexisting inventory slot!");
                return null;
            }

            if (slotIndex < inventory.Count)
                return ItemDatabase.Instance.GetItemByID(inventory[slotIndex]);

            return null;
        }

        public Action<(int changedSlot, InventoryItemSO newItem)> OnInventorySlotChange;
        public Action<Inventory> OnInventoryUpdate;
        public Action<InventoryItemSO> OnItemAdded;
        public Action<InventoryItemSO> OnItemRemoved;

        public int SelectedSlotIndex { get; private set; }
        public Action<int> OnSelectedSlotIndexChanged;
        
        public void ChangeSelectedSlotIndex(int newSelectedSlotIndex)
        {
            newSelectedSlotIndex = newSelectedSlotIndex % InventorySize;

            SelectedSlotIndex = newSelectedSlotIndex;
            OnSelectedSlotIndexChanged?.Invoke(SelectedSlotIndex);
        }
        public void MoveSelectedSlotIndexToLeft() => ChangeSelectedSlotIndex(SelectedSlotIndex - 1);
        public void MoveSelectedSlotIndexToRight() => ChangeSelectedSlotIndex(SelectedSlotIndex + 1);

        public override void OnNetworkSpawn()
        {
            inventory.OnListChanged += OnInventoryChanged;

            NetworkPlayerInstance.OwnerInstance.PlayerInputs.Inputs.Player.UseItem.performed += UseSelectedItem;
        }

        public override void OnNetworkDespawn()
        {
            inventory.OnListChanged -= OnInventoryChanged;
        }

        #region Inventory Management
        private void OnInventoryChanged(NetworkListEvent<int> changeEvent)
        {
            switch (changeEvent.Type)
            {
                // When adding an item through List.Add
                case NetworkListEvent<int>.EventType.Add:
                    OnItemAdded?.Invoke(ItemDatabase.Instance.GetItemByID(changeEvent.Value));
                    OnInventorySlotChange?.Invoke((changeEvent.Index, ItemDatabase.Instance.GetItemByID(changeEvent.Value)));
                    break;

                // When changing slot on an existing place
                case NetworkListEvent<int>.EventType.Value:

                    // Check if the item changed at all
                    if (changeEvent.Value == changeEvent.PreviousValue || (changeEvent.Value < 0 && changeEvent.PreviousValue < 0))
                        break;

                    // Checks if item has been added to an empty slot
                    else if (changeEvent.PreviousValue < 0 && changeEvent.Value >= 0)
                        OnItemAdded?.Invoke(ItemDatabase.Instance.GetItemByID(changeEvent.Value));

                    // Checks if item has been removed from a slot
                    else if (changeEvent.PreviousValue >= 0 && changeEvent.Value < 0)
                        OnItemRemoved?.Invoke(ItemDatabase.Instance.GetItemByID(changeEvent.PreviousValue));

                    OnInventorySlotChange?.Invoke((changeEvent.Index, ItemDatabase.Instance.GetItemByID(changeEvent.Value)));
                    break;

                // For more complex operations it is safer to just force update the whole inventory
                default:
                    OnInventoryUpdate?.Invoke(this);
                    break;
            }
        }

        public bool AddItem(InventoryItemSO item)
        {
            if (item == null)
                return false;

            return AddItem(item.ItemID);
        }
        public bool AddItem(int itemID)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] >= 0)
                    continue;

                inventory[i] = itemID;
                return true;
            }

            if(inventory.Count < InventorySize)
            {
                inventory.Add(itemID);
                return true;
            }

            return false;
        }

        public bool RemoveItem(InventoryItemSO item)
        {
            if(item == null)
                return false;

            return RemoveItem(item.ItemID);
        }
        public bool RemoveItem(int itemID)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] != itemID)
                    continue;

                inventory[i] = -1;
                return true;
            }

            return false;
        }
        #endregion

        private void UseSelectedItem(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;

            if (!IsOwner)
                return;

            InventoryItemSO item = GetItemAtSlot(SelectedSlotIndex);
            if (item == null)
                return;

            Vector2Int cellPosition = NetworkPlayerInstance.OwnerInstance.GridInteractor.selectedCell.Value;

            item.UseItem(cellPosition);
        }
    }
}