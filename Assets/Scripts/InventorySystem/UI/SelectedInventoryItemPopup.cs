using TMPro;
using UnityEngine;

namespace InventorySystem.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SelectedInventoryItemPopup : MonoBehaviour
    {
        private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private AnimationCurve alphaOverTime = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        [SerializeField] private float popupTime = 2.5f;
        private float popupTimer;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();

            NetworkPlayerInstance.OnOwnerInstanceAssigned += OnAssigned;
            NetworkPlayerInstance.OnOwnerInstanceUnassigned += OnUnassigned;

            if (NetworkPlayerInstance.IsOwnerAssigned)
                OnAssigned(NetworkPlayerInstance.OwnerInstance);
        }

        private void Update()
        {
            HandlePopupUpdate();
        }

        private void HandlePopupUpdate(int _ = 0)
        {
            if (popupTimer >= popupTime)
                return;

            popupTimer += Time.deltaTime;
            canvasGroup.alpha = alphaOverTime.Evaluate(Mathf.Clamp01(popupTimer / popupTime));
        }

        private void OnSlotChanged(int _ = 0) => SetItemPopup(NetworkPlayerInstance.OwnerInstance.Inventory.GetSelectedItem());

        private void OnAddedItem((int slotChanged, InventoryItemSO newItem) values)
        {
            if (values.slotChanged != NetworkPlayerInstance.OwnerInstance.Inventory.SelectedSlotIndex)
                return;

            SetItemPopup(values.newItem);
        }

        private void SetItemPopup(InventoryItemSO inventoryItem)
        {
            if(inventoryItem == null)
            {
                ClearItemPopup();
                return;
            }

            popupTimer = 0;
            text.SetText(inventoryItem.ItemName);
        }

        private void ClearItemPopup()
        {
            popupTimer = popupTime;
            canvasGroup.alpha = 0;
        }

        private void OnEnable()
        {
            if (NetworkPlayerInstance.IsOwnerAssigned)
                OnSlotChanged();
            else
                ClearItemPopup();
        }

        private void OnAssigned(NetworkPlayerInstance networkPlayer)
        {
            networkPlayer.Inventory.OnSelectedSlotIndexChanged += OnSlotChanged;
            networkPlayer.Inventory.OnInventorySlotChange += OnAddedItem;
        }

        private void OnUnassigned(NetworkPlayerInstance networkPlayer)
        {
            networkPlayer.Inventory.OnSelectedSlotIndexChanged -= OnSlotChanged;
            networkPlayer.Inventory.OnInventorySlotChange -= OnAddedItem;
        }

        private void OnDestroy()
        {
            NetworkPlayerInstance.OnOwnerInstanceAssigned -= OnAssigned;
            NetworkPlayerInstance.OnOwnerInstanceUnassigned -= OnUnassigned;

            if (NetworkPlayerInstance.IsOwnerAssigned)
                OnUnassigned(NetworkPlayerInstance.OwnerInstance);
        }
    }
}