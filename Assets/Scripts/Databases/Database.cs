using InventorySystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Databases
{
    [Serializable]
    public class Database<T> where T : ScriptableObject, IDataIdentifiable<T>
    {
        public Type Type => typeof(T);

        [SerializeField] private T[] includedData = new T[0];
        private Dictionary<int, T> dataCollection;

        public void Initialize()
        {
            dataCollection = new Dictionary<int, T>();
            foreach (var data in includedData)
            {
                if (data is not IDataIdentifiable<T> dataIdentifier)
                    continue;

                if (dataCollection.TryAdd(dataIdentifier.DataID, data))
                    continue;

                Debug.LogError($"Error: multiple data with ID {dataIdentifier.DataID}! Skipping to next...");
                continue;
            }
        }

        public int GetDataID(T data)
        {
            for (int i = 0; i < includedData.Length; i++)
            {
                if (includedData[i] != data)
                    continue;

                return i;
            }

            return -1;
        }

        public T GetDataByID(int id)
        {
            if (dataCollection.TryGetValue(id, out T data))
            {
                return data;
            }
            Debug.LogError($"Data ID {id} not found in database!");
            return null;
        }
    }
}