using UnityEngine;

namespace UI.Mobile
{
    public class MobileUI : MonoBehaviour
    {
        [SerializeField] private bool alwaysShow;

        private void Awake()
        {
            gameObject.SetActive(alwaysShow || Application.isMobilePlatform);
        }
    }
}