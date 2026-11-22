using Databases;
using InventorySystem;
using UnityEngine;

namespace GameMechanics.Farming
{
    [CreateAssetMenu(menuName = "Plant")]
    public class PlantSO : ScriptableObject, IDataIdentifiable<PlantSO>
    {        
        int IDataIdentifiable<PlantSO>.DataID { get => PlantID; set { } }
        [field: SerializeField, Min(0)] public int PlantID { get; private set; } = 1;

        [field: SerializeField] public string PlantName { get; private set; }
        [field: SerializeField] public Sprite PlantSprite { get; set; }

        [field: SerializeField] public int GrowthTime { get; private set; }

        // TODO this will be instantiated visual object for showing growth progress
        [field: SerializeField] public GrowthVisuals GrowthProgressObject { get; private set; }

        [Header("Rewards")]
        [SerializeField] private InventoryItemSO[] itemDrops = new InventoryItemSO[0];
        [SerializeField] private int minDrops = 2;
        [SerializeField] private int maxDrops = 3;

        public InventoryItemSO[] GetItemDrops() => itemDrops;
        public InventoryItemSO[] GetRandomDrops()
        {
            int totalDrops = Random.Range(minDrops, maxDrops);

            InventoryItemSO[] drops = new InventoryItemSO[totalDrops];
            for (int i = 0; i < drops.Length; i++)
            {
                drops[i] = itemDrops[Random.Range(0, itemDrops.Length)];
            }

            return drops;
        }
    }
}