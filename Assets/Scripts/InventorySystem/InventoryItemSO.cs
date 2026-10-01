using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(menuName = "InventoryItem")]
    public class InventoryItemSO : ScriptableObject
    {
        [field: SerializeField, Min(0)] public int ItemID { get; private set; } = 1;

        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public Sprite ItemSprite { get; private set; }
    }
}