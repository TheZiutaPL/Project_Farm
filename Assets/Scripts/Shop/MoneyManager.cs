using System;
using Unity.Netcode;
using UnityEngine;

namespace Shop
{
    public class MoneyManager : NetworkBehaviour
    {
        public static MoneyManager Instance;
        private NetworkVariable<int> money = new NetworkVariable<int>();

        public static Action<int> OnMoneyChanged;
        public static int GetMoney() => Instance != null ? Instance.money.Value : 0;

        public bool HasMoney(int money) => GetMoney() >= money;

        public override void OnNetworkSpawn()
        {
            if (Instance == null)
            {
                Instance = this;

                OnMoneyChanged?.Invoke(money.Value);
                money.OnValueChanged += MoneyChanged;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (Instance == this)
                money.OnValueChanged -= MoneyChanged;
        }

        private void MoneyChanged(int previousValue, int newValue)
        {
            OnMoneyChanged?.Invoke(newValue);
        }

        public void AddMoney(int value)
        {
            if (value < 0)
                return;

            money.Value += value;
        }

        public void RemoveMoney(int value)
        {
            if (value < 0)
                return;

            money.Value = Mathf.Max(money.Value - value, 0);
        }
    }
}