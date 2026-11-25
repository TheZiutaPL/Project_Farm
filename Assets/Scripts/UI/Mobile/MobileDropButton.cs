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
        }

        protected override void OnUnassignePlayer(NetworkPlayerInstance player)
        {
            if (player.Inventory == null)
                return;

            player.Inventory.OnSelectedSlotIndexChanged -= OnSelectedSlotChanged;
        }

        private void OnSelectedSlotChanged(int slotIndex) => Refresh();
    }
}