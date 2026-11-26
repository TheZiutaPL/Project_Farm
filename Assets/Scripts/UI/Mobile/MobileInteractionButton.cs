using InteractionSystem;

namespace UI.Mobile
{
    public class MobileInteractionButton : MobileButton
    {
        protected override bool IsActive() =>
            NetworkPlayerInstance.IsOwnerAssigned &&
            NetworkPlayerInstance.OwnerInstance.Interactor != null &&
            NetworkPlayerInstance.OwnerInstance.Interactor.IsInteractionPossible;

        protected override void OnAssignedPlayer(NetworkPlayerInstance player)
        {
            if (player.Interactor == null)
                return;

            player.Interactor.OnTargetChanged += OnTargetChanged;
        }

        protected override void OnUnassignePlayer(NetworkPlayerInstance player)
        {
            if (player.Interactor == null)
                return;

            player.Interactor.OnTargetChanged -= OnTargetChanged;
        }

        private void OnTargetChanged(ITargetable targetable) => Refresh();
    }
}