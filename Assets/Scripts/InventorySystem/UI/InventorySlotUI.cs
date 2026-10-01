using UnityEngine;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public class InventorySlotUI : MonoBehaviour
    {
        private InventoryItemSO item;

        [SerializeField] private Sprite emptySlotSprite;
        [SerializeField] private Image itemImage;

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
    }
}