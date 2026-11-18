using InventorySystem;
using RuntimeGrid;
using System;
using UnityEngine;

namespace GameMechanics.Farming
{
    [CreateAssetMenu(menuName = "Items/WateringItem")]
    public class WateringItemSO : InventoryItemSO
    {
        public override Action<NetworkPlayerInstance, RuntimeCell> GetItemUseAction()
        {
            Action<NetworkPlayerInstance, RuntimeCell> action = base.GetItemUseAction();

            action += (player, cell) =>
            {
                if (cell.IsEmpty)
                    return;

                if (cell.GridContent is not FarmGridContent farm)
                    return;

                farm.WaterFarmRpc();
            };

            return action;
        }
    }
}