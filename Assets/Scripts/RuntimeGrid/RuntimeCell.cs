using UnityEngine;

namespace RuntimeGrid
{
    [System.Serializable]
    public class RuntimeCell
    {
        [SerializeField] private GameObject runtimeCellObject;
        public bool IsEmpty => runtimeCellObject == null;
    }
}