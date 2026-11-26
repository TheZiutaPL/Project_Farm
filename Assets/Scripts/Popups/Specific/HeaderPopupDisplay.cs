using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Popups.Specific
{
    public class HeaderPopupDisplay : PopupDisplay
    {
        [SerializeField] private TextMeshProUGUI popupNameText;

        protected override void SetPopupInfo(string popupName, Dictionary<string, object> popupInfo)
        {
            popupNameText.SetText(popupName);
        }
    }
}