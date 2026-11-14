using UnityEngine;
using InventorySystem;
using UnityEngine.UI;

namespace InventorySystem.UI
{
    public class ItemDisplay : MonoBehaviour
    {
        [SerializeField] private Sprite emptyDisplaySprite;
        [SerializeField] private Image itemImage;

        public void SetDisplay(InventoryItemSO item)
        {
            if (item == null)
            {
                ClearDisplay();
                return;
            }

            itemImage.sprite = item.ItemSprite;
        }

        public void ClearDisplay()
        {
            itemImage.sprite = emptyDisplaySprite;
        }
    }
}