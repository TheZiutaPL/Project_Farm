using RuntimeGrid;
using UnityEngine;

namespace InventorySystem.ItemTypes
{
    [CreateAssetMenu(menuName = "Items/BuildingItem")]
    public class BuildingItemSO : InventoryItemSO
    {
        [SerializeField] private GameObject buildingObject;
        [SerializeField] private bool removeItemOnUse = true;

        public override void UseItem(Vector2Int cellPosition)
        {
            base.UseItem(cellPosition);

            if (!RuntimeGridInstance.Instance.TryGetRuntimeCell(cellPosition, out RuntimeCell cell))
            {
                Debug.LogWarning("Used item on an invalid cell");
                return;
            }

            if (!cell.IsEmpty)
            {
                Debug.LogWarning("Cell is not empty");
                return;
            }

            // TODO this will not work with netcode, also will need to be spawned via Rpc | possibly most of this method should be in Rpc
            GameObject building = Instantiate(buildingObject, RuntimeGridInstance.Instance.CellToWorldPosition(cellPosition), Quaternion.identity);
            cell.SetCellObject(building);

            // TODO This also should be via Rpc, since clients cannot change inventories
            if (removeItemOnUse)
                NetworkPlayerInstance.OwnerInstance.Inventory.RemoveItem(ItemID);
        }
    }
}