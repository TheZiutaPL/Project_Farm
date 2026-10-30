using Databases;
using GameTime;
using RuntimeGrid;
using Unity.Netcode;
using UnityEngine;

namespace GameMechanics.Farming
{
    public class FarmGridContent : GridContent
    {
        private PlantSO crop;
        private NetworkVariable<float> growTime = new NetworkVariable<float>();
        private GrowthVisuals growthVisuals;

        public bool IsPlanted => crop != null;

        public bool IsReady => IsPlanted && growTime.Value >= crop.GrowthTime;

        public bool CanPlant => !IsPlanted;

        public override void OnNetworkSpawn()
        {
            GameTimeManager.OnGameTimePassed += OnTimePassed;
        }

        public override void OnNetworkDespawn()
        {
            GameTimeManager.OnGameTimePassed -= OnTimePassed;
        }

        public void SetPlantRpc(int plantID)
        {
            PlantSO plantSO = GameDatabase.Instance.PlantDatabase.GetDataByID(plantID);
            growTime.Value = 0;

            SetupPlantForUsersRpc(plantID);
        }

        [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
        private void SetupPlantForUsersRpc(int plantID)
        {
            PlantSO plantSO = GameDatabase.Instance.PlantDatabase.GetDataByID(plantID);

            crop = plantSO;
            growthVisuals = GameObject.Instantiate(plantSO.GrowthProgressObject, transform);
        }

        private void OnTimePassed(float minutesPassed)
        {
            if (!IsPlanted)
                return;

            if (IsServer)
                growTime.Value += minutesPassed;

            if (growthVisuals != null)
            {
                float progress01 = Mathf.Clamp01(crop.GrowthTime > 0 ? growTime.Value / crop.GrowthTime : 1);
                growthVisuals.UpdateGrowthStage(progress01);
            }
        }
    }
}