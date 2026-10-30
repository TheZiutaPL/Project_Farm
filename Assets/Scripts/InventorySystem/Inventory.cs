using Databases;
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

            if (slotIndex < inventory.Count && inventory[slotIndex] >= 0)
                return GameDatabase.Instance.ItemDatabase.GetDataByID(inventory[slotIndex]);

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
            newSelectedSlotIndex = (InventorySize + newSelectedSlotIndex) % InventorySize;

            SelectedSlotIndex = newSelectedSlotIndex;
            OnSelectedSlotIndexChanged?.Invoke(SelectedSlotIndex);
        }
        public void MoveSelectedSlotIndexToLeft() => ChangeSelectedSlotIndex(SelectedSlotIndex - 1);
        public void MoveSelectedSlotIndexToRight() => ChangeSelectedSlotIndex(SelectedSlotIndex + 1);

        public override void OnNetworkSpawn()
        {
            inventory.OnListChanged += OnInventoryChanged;

            if (IsOwner)
            {
                PlayerInputs.Instance.Inputs.Player.UseItem.performed += UseSelectedItem;

                PlayerInputs.Instance.Inputs.Player.SlotSelection.performed += ChangeSelectedSlot;
            }
        }

        public override void OnNetworkDespawn()
        {
            inventory.OnListChanged -= OnInventoryChanged;

            if (IsOwner)
            {
                PlayerInputs.Instance.Inputs.Player.UseItem.performed -= UseSelectedItem;

                PlayerInputs.Instance.Inputs.Player.SlotSelection.performed -= ChangeSelectedSlot;
            }
        }

        #region Inventory Management
        private void OnInventoryChanged(NetworkListEvent<int> changeEvent)
        {
            switch (changeEvent.Type)
            {
                // When adding an item through List.Add
                case NetworkListEvent<int>.EventType.Add:
                    OnItemAdded?.Invoke(GameDatabase.Instance.ItemDatabase.GetDataByID(changeEvent.Value));
                    OnInventorySlotChange?.Invoke((changeEvent.Index, GameDatabase.Instance.ItemDatabase.GetDataByID(changeEvent.Value)));
                    break;

                // When changing slot on an existing place
                case NetworkListEvent<int>.EventType.Value:

                    // Check if the item changed at all
                    if (changeEvent.Value == changeEvent.PreviousValue || (changeEvent.Value < 0 && changeEvent.PreviousValue < 0))
                        break;

                    // Checks if item has been added to an empty slot
                    else if (changeEvent.PreviousValue < 0 && changeEvent.Value >= 0)
                        OnItemAdded?.Invoke(GameDatabase.Instance.ItemDatabase.GetDataByID(changeEvent.Value));

                    // Checks if item has been removed from a slot
                    else if (changeEvent.PreviousValue >= 0 && changeEvent.Value < 0)
                        OnItemRemoved?.Invoke(GameDatabase.Instance.ItemDatabase.GetDataByID(changeEvent.PreviousValue));

                    
                    OnInventorySlotChange?.Invoke((changeEvent.Index, changeEvent.Value < 0 ? null : GameDatabase.Instance.ItemDatabase.GetDataByID(changeEvent.Value)));
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

        private void ChangeSelectedSlot(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed)
                return;

            float axis = ctx.ReadValue<float>();

            if (axis == 0)
                return;

            if (axis > 0)
                MoveSelectedSlotIndexToLeft();
            else
                MoveSelectedSlotIndexToRight();
        }

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