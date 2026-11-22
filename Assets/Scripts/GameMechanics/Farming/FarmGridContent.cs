using Databases;
using GameTime;
using InteractionSystem;
using InventorySystem;
using Popups;
using Popups.Specific;
using RuntimeGrid;
using StatTracking;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GameMechanics.Farming
{
    public class FarmGridContent : GridContent, IInteractable, IPopable
    {

        private PlantSO plant;
        private NetworkVariable<float> growTime = new NetworkVariable<float>();
        private GrowthVisuals growthVisuals;

        public bool IsPlanted => plant != null;

        public bool IsReady => IsPlanted && growTime.Value >= plant.GrowthTime;
        public float GetGrowth01() => IsPlanted ? (plant.GrowthTime > 0 ? Mathf.Clamp01(growTime.Value / plant.GrowthTime) : 1) : 0;

        public bool CanPlant => !IsPlanted;

        [SerializeField] private MeshRenderer farmlandMesh;
        private const string SHADER_WATERING_EFFECT_KEY = "_WaterFactor";

        [SerializeField] private float fullWaterTime = 60;
        [SerializeField] private float wateringMultiplier = 2f;
        private NetworkVariable<float> waterTime = new NetworkVariable<float>();
        public float GetWater01() => fullWaterTime > 0 ? Mathf.Clamp01(waterTime.Value / fullWaterTime) : 0;

        public bool IsWatered => waterTime.Value >= 0;

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

            StatTrackingManager.Instance.IncrementStat(GameStats.CROPS_PLANTED);

            SetupPlantForUsersRpc(plantID);
        }

        [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Server)]
        private void SetupPlantForUsersRpc(int plantID)
        {
            PlantSO plantSO = GameDatabase.Instance.PlantDatabase.GetDataByID(plantID);

            plant = plantSO;
            growthVisuals = GameObject.Instantiate(plantSO.GrowthProgressObject, transform);
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        public void ClearPlantRpc()
        {
            if (!IsPlanted)
                return;

            if (IsReady)
            {
                ItemSpawnManager.SpawnItems(transform.position, plant.GetRandomDrops());
                StatTrackingManager.Instance.IncrementStat(GameStats.CROPS_GATHERED);
            }

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

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Server)]
        public void WaterFarmRpc()
        {
            waterTime.Value = fullWaterTime;
        }

        private void OnTimePassed(float minutesPassed)
        {
            if (IsServer)
            {
                if (IsPlanted && !IsReady)
                {
                    float multiplier = IsWatered ? wateringMultiplier : 1;
                    growTime.Value += minutesPassed * multiplier;
                }

                waterTime.Value -= minutesPassed;
            }

            if (IsPlanted && growthVisuals != null)
            {
                float progress01 = GetGrowth01();
                growthVisuals.UpdateGrowthStage(progress01);
            }

            farmlandMesh.material.SetFloat(SHADER_WATERING_EFFECT_KEY, GetWater01());
        }
        public void Interact()
        {
            if (!IsReady)
                return;

            ClearPlantRpc();
        }

        public Action<bool> OnTargetAction { get; set; }

        public string PopupName => IsPlanted ? plant.PlantName : $"Empty";

        public PopupType PopupType => PopupType.Plant;

        public Vector3 Position => transform.position + Vector3.up;

        private PopupInstance popupInstance;

        public void OnTarget()
        {
            if(IsPlanted)
                popupInstance = PopupUI.SetPopup(this);
        }

        public void OffTarget()
        {
            popupInstance.ReleasePopup();
        }

        public Dictionary<string, object> GetPopupInfo() => new Dictionary<string, object>()
        {
            { PopupInfoKeywords.PLANT_SPRITE, plant != null ? plant.PlantSprite : null },
            { PopupInfoKeywords.SOIL_MOISTURE_01, GetWater01() },
            { PopupInfoKeywords.PLANT_GROWTH_01, GetGrowth01() },
        };
    }
}