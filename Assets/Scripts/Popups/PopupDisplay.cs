using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Popups
{
    public abstract class PopupDisplay : MonoBehaviour
    {
        private IPopable popable;

        public bool IsUsed => gameObject.activeSelf;

        [SerializeField] private float refreshTime = .6f;
        private float refreshTimer = 0f;

        public void SetPopup(IPopable popup)
        {
            popable = popup;
            gameObject.SetActive(UtilityMethods.IsInterfaceValid(popup));

            refreshTimer = refreshTime;

            HandlePositionUpdate();
            RefreshPopup();
        }

        public void ReleasePopup()
        {
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (popable == null)
                return;

            if(!UtilityMethods.IsInterfaceValid(popable))
            {
                popable = null;
                return;
            }

            HandleRefreshUpdate();

            HandlePositionUpdate();
        }

        private void HandleRefreshUpdate()
        {
            refreshTimer -= Time.deltaTime;

            if (refreshTimer < 0f)
            {
                RefreshPopup();
                refreshTimer = refreshTime;
            }
        }

        private void HandlePositionUpdate()
        {
            transform.position = NetworkPlayerInstance.OwnerInstance.PlayerCameraHandler.PlayerCamera.WorldToScreenPoint(popable.Position);
        }

        protected void RefreshPopup()
        {
            if (popable == null || !UtilityMethods.IsInterfaceValid(popable))
            {
                ReleasePopup();
                return;
            }

            SetPopupInfo(popable.PopupName, popable.GetPopupInfo());
        }
        protected abstract void SetPopupInfo(string popupName, Dictionary<string, object> popupInfo);
    }
}
