using UnityEngine;

namespace RuntimeGrid
{
    [System.Serializable]
    public class RuntimeCell
    {
        public Vector2Int CellPosition { get; private set; }

        [SerializeField] private GridContent runtimeCellObject;
        public bool IsEmpty => runtimeCellObject == null;

        public RuntimeCell(Vector2Int cellPosition)
        {
            CellPosition = cellPosition;
        }

        public void SetCellObject(GridContent cellObject)
        {
            runtimeCellObject = cellObject;
        }
    }
}