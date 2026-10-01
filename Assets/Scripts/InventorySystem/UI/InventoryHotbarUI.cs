using System;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.UI
{
    public class InventoryHotbarUI : MonoBehaviour
    {
        [SerializeField] private InventorySlotUI inventorySlotPrefab;
        [SerializeField] private Transform inventorySlotContainer;
        private List<InventorySlotUI> slots = new List<InventorySlotUI>();

        private void OnEnable()
        {
            NetworkPlayerInstance.OnOwnerInstanceAssigned += OnOwnerInstanceAssigned;
            NetworkPlayerInstance.OnOwnerInstanceUnassigned += OnOwnerInstanceUnassigned;

            if (NetworkPlayerInstance.IsOwnerAssigned)
                ForceInventoryUpdate(NetworkPlayerInstance.OwnerInstance.Inventory);
        }

        private void OnDisable()
        {
            NetworkPlayerInstance.OnOwnerInstanceAssigned -= OnOwnerInstanceAssigned;
            NetworkPlayerInstance.OnOwnerInstanceUnassigned -= OnOwnerInstanceUnassigned;
        }

        private void OnOwnerInstanceAssigned(NetworkPlayerInstance instance)
        {
            // TODO no inventory scenario
            if (instance.Inventory == null)
                return;

            instance.Inventory.OnInventorySlotChange += OnInventorySlotChanged;
            instance.Inventory.OnInventoryUpdate += ForceInventoryUpdate;

            ForceInventoryUpdate(instance.Inventory);
        }

        private void OnOwnerInstanceUnassigned(NetworkPlayerInstance instance)
        {
            if (instance.Inventory == null)
                return;

            instance.Inventory.OnInventorySlotChange -= OnInventorySlotChanged;
            instance.Inventory.OnInventoryUpdate -= ForceInventoryUpdate;
        }

        private void OnInventorySlotChanged((int changedSlot, InventoryItemSO newItem) change)
        {
            slots[change.changedSlot].SetItem(change.newItem);
        }

        private void ForceInventoryUpdate(Inventory inventory)
        {
            UpdateSize(inventory.InventorySize);

            for (int i = 0; i < slots.Count; i++)
                slots[i].SetItem(inventory.GetItemAtSlot(i));
        }

        private void UpdateSize(int newSize)
        {
            if (newSize == slots.Count)
                return;

            if(newSize > slots.Count)
            {
                int missing = newSize - slots.Count;
                for (int i = 0; i < missing; i++)
                    slots.Add(Instantiate(inventorySlotPrefab, inventorySlotContainer));
            }
            else
            {
                int toDelete = slots.Count - newSize;
                for (int i = 0; i < toDelete; i++)
                {
                    Destroy(slots[^(i + 1)].gameObject);
                    slots.RemoveAt(slots.Count - 1);
                }
            }
        }
    }
}