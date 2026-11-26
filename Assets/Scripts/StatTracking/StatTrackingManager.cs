using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace StatTracking
{
    public class StatTrackingManager : NetworkBehaviour
    {
        public static StatTrackingManager Instance { get; private set; }

        private NetworkList<GameStatUnit> statsUnits = new NetworkList<GameStatUnit>
            (
            new GameStatUnit[]
            {
                new GameStatUnit(GameStats.REQUESTS_COMPLETED),
                new GameStatUnit(GameStats.REQUEST_ITEMS_SUBMITTED),

                new GameStatUnit(GameStats.ITEMS_BOUGHT),

                new GameStatUnit(GameStats.MONEY_GATHERED),
                new GameStatUnit(GameStats.MONEY_SPENT),

                new GameStatUnit(GameStats.CROPS_PLANTED),
                new GameStatUnit(GameStats.CROPS_GATHERED),
            }
            );
        private bool FindStatUnitIndex(GameStats stat, out int index)
        {
            index = FindStatUnitIndex(stat);

            return index >= 0;
        }
        private int FindStatUnitIndex(GameStats stat)
        {
            for (int i = 0; i < statsUnits.Count; i++)
            {
                if (statsUnits[i].gameStat != stat)
                    continue;

                return i;
            }

            return -1;
        }

        public static Action<GameStats, int> OnStatUpdated;

        private void Awake()
        {
            if(Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }

            for (int i = 0; i < statsUnits.Count; i++)
                OnStatUpdated?.Invoke(statsUnits[i].gameStat, statsUnits[i].statValue);
        }

        public void IncrementStat(GameStats stat, bool decrement = false)
        {
            if (!FindStatUnitIndex(stat, out int index))
                return;

            GameStatUnit current = statsUnits[index];

            if(!decrement)
                current.statValue++;
            else
                current.statValue--;

            statsUnits[index] = current;

            StatUpdateRpc(current);
        }
        public void ChangeStat(GameStats stat, int change)
        {
            if (!FindStatUnitIndex(stat, out int index))
                return;

            GameStatUnit current = statsUnits[index];
            current.statValue += change;

            statsUnits[index] = current;

            StatUpdateRpc(current);
        }

        [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
        private void StatUpdateRpc(params GameStatUnit[] updatedStats)
        {
            for (int i = 0; i < updatedStats.Length; i++)
                OnStatUpdated?.Invoke(updatedStats[i].gameStat, updatedStats[i].statValue);
        }

        public int GetStat(GameStats stat)
        {
            if (!FindStatUnitIndex(stat, out int index))
                return -1;

            return statsUnits[index].statValue;
        }

        public string GetStatString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            int index;

            if(FindStatUnitIndex(GameStats.REQUESTS_COMPLETED, out index))
                stringBuilder.AppendLine($"Requests completed: {statsUnits[index].statValue}");
            if (FindStatUnitIndex(GameStats.REQUEST_ITEMS_SUBMITTED, out index))
                stringBuilder.AppendLine($"Requests items delivered: {statsUnits[index].statValue}");

            if (FindStatUnitIndex(GameStats.ITEMS_BOUGHT, out index))
                stringBuilder.AppendLine($"Items bought: {statsUnits[index].statValue}");

            if (FindStatUnitIndex(GameStats.MONEY_GATHERED, out index))
                stringBuilder.AppendLine($"Money gathered: {statsUnits[index].statValue}");
            if (FindStatUnitIndex(GameStats.MONEY_SPENT, out index))
                stringBuilder.AppendLine($"Money spent: {statsUnits[index].statValue}");

            if (FindStatUnitIndex(GameStats.CROPS_PLANTED, out index))
                stringBuilder.AppendLine($"Crops planted: {statsUnits[index].statValue}");
            if (FindStatUnitIndex(GameStats.CROPS_GATHERED, out index))
                stringBuilder.AppendLine($"Crops gathered: {statsUnits[index].statValue}");

            return stringBuilder.ToString();
        }
    }
}