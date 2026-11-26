using InventorySystem;
using RuntimeGrid;
using System;
using UnityEngine;

namespace GameMechanics.Building
{
    [CreateAssetMenu(menuName = "Items/BuildingItem")]
    public class BuildingItemSO : InventoryItemSO
    {
        [SerializeField] private GridContent buildingObject;
        [SerializeField] private bool removeItemOnUse = true;

        public override void ItemUseServerAction(NetworkPlayerInstance player, RuntimeCell cell)
        {
            base.ItemUseServerAction(player, cell);

            if (!cell.IsEmpty)
                return;

            GridContent building = Instantiate(buildingObject, RuntimeGridInstance.Instance.CellToWorldPosition(cell.CellPosition), Quaternion.identity);
            building.NetworkObject.Spawn(true);

            building.BindToGridRpc(cell.CellPosition);

            if (removeItemOnUse)
                player.Inventory.RemoveItemWithSelectedPriority(ItemID);
        }
    }
}