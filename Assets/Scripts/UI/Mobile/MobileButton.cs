using UnityEngine;

namespace UI.Mobile
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class MobileButton : MonoBehaviour
    {
        private CanvasGroup canvasGroup;

        protected virtual bool IsActive() => true;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            NetworkPlayerInstance.OnOwnerInstanceAssigned += OnAssignedPlayer;
            NetworkPlayerInstance.OnOwnerInstanceUnassigned += OnUnassignePlayer;

            if (NetworkPlayerInstance.IsOwnerAssigned)
                OnAssignedPlayer(NetworkPlayerInstance.OwnerInstance);
        }

        private void OnDestroy()
        {
            NetworkPlayerInstance.OnOwnerInstanceAssigned -= OnAssignedPlayer;
            NetworkPlayerInstance.OnOwnerInstanceUnassigned -= OnUnassignePlayer;

            if(NetworkPlayerInstance.IsOwnerAssigned)
                OnUnassignePlayer(NetworkPlayerInstance.OwnerInstance);
        }

        public void Refresh()
        {
            bool isActive = IsActive();

            canvasGroup.alpha = isActive ? 1 : .6f;

            canvasGroup.interactable = isActive;
            canvasGroup.blocksRaycasts = isActive;
        }

        protected virtual void OnAssignedPlayer(NetworkPlayerInstance player)
        { }

        protected virtual void OnUnassignePlayer(NetworkPlayerInstance player)
        { }
    }
}