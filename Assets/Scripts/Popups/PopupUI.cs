using System.Collections.Generic;
using UnityEngine;

namespace Popups
{
    public class PopupUI : MonoBehaviour
    {
        private static PopupUI instance;

        private Dictionary<PopupType, List<PopupDisplay>> popupPools;

        [Header("Popup Types")]
        [SerializeField] private PopupDisplay headerPopupPrefab; 
        [SerializeField] private PopupDisplay plantPopupPrefab;
        [SerializeField] private PopupDisplay requestPopupPrefab;
        private PopupDisplay GetPrefabForPopupType(PopupType popupType) => popupType switch
        {
            PopupType.Header => headerPopupPrefab,
            PopupType.Plant => plantPopupPrefab,
            PopupType.Request => requestPopupPrefab,
            _ => throw new System.NotImplementedException(),
        };

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            popupPools = new Dictionary<PopupType, List<PopupDisplay>>();
        }

        public static PopupInstance SetPopup(IPopable popable)
        {
            if(instance == null)
                return default;

            PopupDisplay popup = instance.GetPopupOfType(popable.PopupType);
            popup.SetPopup(popable);

            return new PopupInstance(popup);
        }

        private PopupDisplay GetPopupOfType(PopupType popupType)
        {
            if(!popupPools.TryGetValue(popupType, out List<PopupDisplay> popupList))
                return CreatePopup(popupType);

            for (int i = 0; i < popupList.Count; i++)
            {
                if (popupList[i].IsUsed)
                    continue;

                return popupList[i];
            }

            return CreatePopup(popupType);
        }

        private PopupDisplay CreatePopup(PopupType popupType)
        {
            PopupDisplay popup = Instantiate(GetPrefabForPopupType(popupType), transform);

            if(popupPools.ContainsKey(popupType))
                popupPools[popupType].Add(popup);
            else
                popupPools.Add(popupType, new List<PopupDisplay>() { popup });

            return popup;
        }
    }
}