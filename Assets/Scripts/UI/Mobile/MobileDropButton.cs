using InventorySystem;

namespace UI.Mobile
{
    public class MobileDropButton : MobileButton
    {
        protected override bool IsActive() =>
            NetworkPlayerInstance.IsOwnerAssigned &&
            NetworkPlayerInstance.OwnerInstance.Inventory != null &&
            NetworkPlayerInstance.OwnerInstance.Inventory.GetSelectedItem() != null;

        protected override void OnAssignedPlayer(NetworkPlayerInstance player)
        {
            if (player.Inventory == null)
                return;

            player.Inventory.OnSelectedSlotIndexChanged += OnSelectedSlotChanged;
            player.Inventory.OnInventorySlotChange += OnInventorySlotChange;
            player.Inventory.OnInventoryUpdate += OnInventoryUpdate;
        }

        protected override void OnUnassignePlayer(NetworkPlayerInstance player)
        {
            if (player.Inventory == null)
                return;

            player.Inventory.OnSelectedSlotIndexChanged -= OnSelectedSlotChanged;
            player.Inventory.OnInventorySlotChange -= OnInventorySlotChange;
            player.Inventory.OnInventoryUpdate -= OnInventoryUpdate;
        }

        private void OnSelectedSlotChanged(int slotIndex) => Refresh();
        private void OnInventorySlotChange((int slot, InventoryItemSO item) slotChange) => Refresh();
        private void OnInventoryUpdate(Inventory inventory) => Refresh();
    }
}