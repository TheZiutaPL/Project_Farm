using Databases;
using RuntimeGrid;
using System;
using UnityEngine;

namespace InventorySystem
{
    [CreateAssetMenu(menuName = "Items/Item")]
    public class InventoryItemSO : ScriptableObject, IDataIdentifiable<InventoryItemSO>
    {
        int IDataIdentifiable<InventoryItemSO>.DataID { get => ItemID; set { } }
        [field: SerializeField, Min(0)] public int ItemID { get; private set; } = 1;

        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public Sprite ItemSprite { get; private set; }

        [field: SerializeField] public int BuyPrice { get; private set; }

        public void UseItem(Vector2Int cellPosition)
        {
            RuntimeGridInstance.Instance.UseItemOnGridRpc(cellPosition, ItemID);
        }

        public virtual void ItemUseServerAction(NetworkPlayerInstance player, RuntimeCell cell)
        {
            Debug.Log($"Used item {ItemName} on cell {cell.CellPosition}");
        }
    }
}