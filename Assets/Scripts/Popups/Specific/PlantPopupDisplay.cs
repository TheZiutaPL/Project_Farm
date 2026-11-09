using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Popups.Specific
{
    public class PlantPopupDisplay : PopupDisplay
    {
        [SerializeField] private TextMeshProUGUI popupNameText;
        [SerializeField] private Slider soilMoistureSlider;
        [SerializeField] private Slider plantGrowthSlider;
        [SerializeField] private GameObject growingGroup;
        [SerializeField] private GameObject readyGroup;

        protected override void SetPopupInfo(string popupName, Dictionary<string, object> popupInfo)
        {
            popupNameText.SetText(popupName);

            if (popupInfo == null)
                return;

            if(popupInfo.TryGetValue(PopupInfoKeywords.SOIL_MOISTURE_01, out object moisture))
            {
                float moisture01 = (float)moisture;
                soilMoistureSlider.value = moisture01;
            }

            if(popupInfo.TryGetValue(PopupInfoKeywords.PLANT_GROWTH_01, out object growth))
            {
                float growth01 = (float)growth;
                plantGrowthSlider.value = growth01;

                bool ready = growth01 >= 1;
                growingGroup.SetActive(!ready);
                readyGroup.SetActive(ready);
            }
        }
    }
}