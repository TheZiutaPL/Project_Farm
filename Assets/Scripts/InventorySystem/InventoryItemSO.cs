using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Items/Item")]
    public class InventoryItemSO : ScriptableObject
    {
        [field: SerializeField, Min(0)] public int ItemID { get; private set; } = 1;

        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public Sprite ItemSprite { get; private set; }

        public virtual void UseItem(Vector2Int cellPosition)
        {
            Debug.Log($"Used item {ItemName} on cell {cellPosition}");
        }
    }
}