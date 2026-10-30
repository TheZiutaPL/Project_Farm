using Databases;
using GameTime;
using InteractionSystem;
using InventorySystem;
using RuntimeGrid;
using Unity.Netcode;
using UnityEngine;

namespace GameMechanics.Farming
{
    public class FarmGridContent : GridContent, IInteractable
    {
        private PlantSO plant;
        private NetworkVariable<float> growTime = new NetworkVariable<float>();
        private GrowthVisuals growthVisuals;

        public bool IsPlanted => plant != null;

        public bool IsReady => IsPlanted && growTime.Value >= plant.GrowthTime;

        public bool CanPlant => !IsPlanted;

        public string TargetableName
        {
            get
            {
                string str = "Farmland";
                if (IsPlanted)
                    str += $" {plant.PlantName}";

                return str;
            }
        }
        public override void OnNetworkSpawn()
        {
            GameTimeManager.OnGameTimePassed += OnTimePassed;
        }

        public override void OnNetworkDespawn()
        {
            GameTimeManager.OnGameTimePassed -= OnTimePassed;
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Server)]
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

            plant = plantSO;
            growthVisuals = GameObject.Instantiate(plantSO.GrowthProgressObject, transform);
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Server)]
        public void ClearPlantRpc()
        {
            if (!IsPlanted)
                return;

            ClearPlantForUsersRpc();

            growTime.Value = 0;
        }

        [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
        private void ClearPlantForUsersRpc()
        {
            plant = null;

            if (growthVisuals != null)
                Destroy(growthVisuals.gameObject);
        }

        private void OnTimePassed(float minutesPassed)
        {
            if (!IsPlanted)
                return;

            if (IsServer)
                growTime.Value += minutesPassed;

            if (growthVisuals != null)
            {
                float progress01 = Mathf.Clamp01(plant.GrowthTime > 0 ? growTime.Value / plant.GrowthTime : 1);
                growthVisuals.UpdateGrowthStage(progress01);
            }
        }
        public void Interact()
        {
            if (!IsReady)
                return;

            ItemSpawnManager.SpawnItems(transform.position, plant.GetItemDrops());
            ClearPlantRpc();
        }

        public void OnTarget()
        {
            // TODO Show popup
        }

        public void OffTarget()
        {
            // TODO Hide popup
        }
    }
}