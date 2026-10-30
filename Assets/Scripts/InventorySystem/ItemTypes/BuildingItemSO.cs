using RuntimeGrid;
using System;
using Unity.Netcode;
using UnityEngine;

namespace InventorySystem.ItemTypes
{
    [CreateAssetMenu(menuName = "Items/BuildingItem")]
    public class BuildingItemSO : InventoryItemSO
    {
        [SerializeField] private GridContent buildingObject;
        [SerializeField] private bool removeItemOnUse = true;

        public override Action<NetworkPlayerInstance, RuntimeCell> GetItemUseAction()
        {
            Action<NetworkPlayerInstance, RuntimeCell> action = base.GetItemUseAction();

            action += (player, cell) =>
            {
                if (!cell.IsEmpty)
                    return;

                GridContent building = Instantiate(buildingObject, RuntimeGridInstance.Instance.CellToWorldPosition(cell.CellPosition), Quaternion.identity);
                building.NetworkObject.Spawn(true);

                building.BindToGridRpc(cell.CellPosition);

                if (removeItemOnUse)
                    player.Inventory.RemoveItem(ItemID);
            };

            return action;
        }
    }
}