using TMPro;
using UnityEngine;

namespace Shop
{
    public class MoneyDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moneyText;

        private void OnEnable()
        {
            MoneyManager.OnMoneyChanged += SetMoneyDisplay;
            SetMoneyDisplay(MoneyManager.GetMoney());
        }

        private void OnDisable()
        {
            MoneyManager.OnMoneyChanged -= SetMoneyDisplay;
        }

        private void SetMoneyDisplay(int value)
        {
            moneyText.SetText(value < 1000 ? value.ToString() : "+999");
        }
    }
}