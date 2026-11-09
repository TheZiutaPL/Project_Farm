using UnityEngine;

namespace Popups
{
    public struct PopupInstance
    {
        private PopupDisplay popupDisplay;

        public PopupInstance(PopupDisplay popupDisplay)
        {
            this.popupDisplay = popupDisplay;
        }

        public void ReleasePopup()
        {
            if (popupDisplay == null)
                return;

            popupDisplay.ReleasePopup();
            popupDisplay = null;
        }
    }
}