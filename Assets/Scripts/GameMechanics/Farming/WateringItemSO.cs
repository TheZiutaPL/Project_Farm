using InventorySystem;
using RuntimeGrid;
using System;
using UnityEngine;

namespace GameMechanics.Farming
{
    [CreateAssetMenu(menuName = "Items/WateringItem")]
    public class WateringItemSO : InventoryItemSO
    {
        public override void ItemUseServerAction(NetworkPlayerInstance player, RuntimeCell cell)
        {
            base.ItemUseServerAction(player, cell);

            if (cell.IsEmpty)
                return;

            if (cell.GridContent is not FarmGridContent farm)
                return;

            farm.WaterFarmRpc();
        }
    }
}