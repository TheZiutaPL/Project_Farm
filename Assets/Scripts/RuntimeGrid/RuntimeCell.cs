using UnityEngine;

namespace RuntimeGrid
{
    [System.Serializable]
    public class RuntimeCell
    {
        public Vector2Int CellPosition { get; private set; }
        public GridContent GridContent { get; private set; }

        public bool IsEmpty => GridContent == null;

        public RuntimeCell(Vector2Int cellPosition)
        {
            CellPosition = cellPosition;
        }

        public void SetCellObject(GridContent cellObject)
        {
            GridContent = cellObject;
        }
    }
}