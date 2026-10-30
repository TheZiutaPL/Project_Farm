using GameMechanics.Farming;
using InventorySystem;
using UnityEngine;

namespace Databases
{
    public class GameDatabase : MonoBehaviour
    {
        public static GameDatabase Instance;
        
        [field: SerializeField] public Database<InventoryItemSO> ItemDatabase { get; private set; }
        [field: SerializeField] public Database<PlantSO> PlantDatabase { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            ItemDatabase.Initialize();
            PlantDatabase.Initialize();
        }
    }
}