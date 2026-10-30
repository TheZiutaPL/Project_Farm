using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    [RequireComponent(typeof(Toggle))]
    public class InventorySlotUI : MonoBehaviour
    {
        private InventoryItemSO item;

        public int SlotIndex { get; private set; }

        private Toggle toggle;
        [SerializeField] private Sprite emptySlotSprite;
        [SerializeField] private Image itemImage;

        private bool isSelected;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(ClickToSelect);
        }

        public void SetupSlot(int slotIndex, ToggleGroup toggleGroup)
        {
            SlotIndex = slotIndex;

            toggle.group = toggleGroup;

            SetSelected(NetworkPlayerInstance.OwnerInstance.Inventory.SelectedSlotIndex == slotIndex);
        }

        public void SetItem(InventoryItemSO newItem)
        {
            if (newItem == null)
            {
                ClearSlot();
                return;
            }

            itemImage.sprite = newItem.ItemSprite;

            // TODO if is selected - automatically select new slot content 
        }

        private void ClearSlot()
        {
            itemImage.sprite = emptySlotSprite;

            // TODO if is selected - deselect;
        }

        public void SetSelected(bool selected)
        {
            toggle.SetIsOnWithoutNotify(selected);
        }
        private void ClickToSelect(bool selected)
        {
            if (!selected)
                return;

            NetworkPlayerInstance.OwnerInstance.Inventory.ChangeSelectedSlotIndex(SlotIndex);
        }
    }
}