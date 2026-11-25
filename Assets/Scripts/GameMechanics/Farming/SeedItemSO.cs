using InventorySystem;
using RuntimeGrid;
using System;
using UnityEngine;

namespace GameMechanics.Farming
{
    [CreateAssetMenu(menuName = "Items/SeedItem")]
    public class SeedItemSO : InventoryItemSO
    {
        [SerializeField] public PlantSO plant;

        public override void ItemUseServerAction(NetworkPlayerInstance player, RuntimeCell cell)
        {
            base.ItemUseServerAction(player, cell);

            if (cell.IsEmpty)
                return;

            if (cell.GridContent is not FarmGridContent farm)
                return;

            if (!farm.CanPlant)
                return;

            farm.SetPlantRpc(plant.PlantID);

            player.Inventory.RemoveItemWithSelectedPriority(ItemID);
        }
    }
}