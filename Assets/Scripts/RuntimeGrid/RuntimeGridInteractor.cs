using Unity.Netcode;
using UnityEngine;

namespace RuntimeGrid
{
    public class RuntimeGridInteractor : NetworkBehaviour
    {
        public NetworkVariable<Vector2Int> selectedCell = new NetworkVariable<Vector2Int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [SerializeField] private GameObject selectionObjectPrefab;
        private Transform selectionObject;

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;

            selectionObject = Instantiate(selectionObjectPrefab).transform;

            selectedCell.OnValueChanged += OnChangedCell;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
                return;

            selectedCell.OnValueChanged -= OnChangedCell;
        }

        private void Update()
        {
            if (!IsOwner)
                return;

            HandleMousePointerUpdate();
        }

        private void HandleMousePointerUpdate()
        {
            Camera cam;
            if (NetworkPlayerInstance.OwnerInstance != null)
                cam = NetworkPlayerInstance.OwnerInstance.PlayerCameraHandler.PlayerCamera;
            else
                cam = Camera.main;

            if (cam == null)
                return;

            // Get mouse position
            Vector2 mousePosition = PlayerInputs.Instance.Inputs.Player.PointerPosition.ReadValue<Vector2>();

            // Get world point position
            Ray ray = cam.ScreenPointToRay(mousePosition);
            RuntimeGridInstance.Instance.GetGridPlane().Raycast(ray, out float distance);
            Vector3 worldPointPosition = ray.GetPoint(distance);

            // Calculate cell position
            Vector2Int currentSelected = RuntimeGridInstance.Instance.WorldToCellPosition(worldPointPosition);

            if (currentSelected != selectedCell.Value)
                selectedCell.Value = currentSelected;
        }

        private void OnChangedCell(Vector2Int previousValue, Vector2Int newValue)
        {
            selectionObject.position = RuntimeGridInstance.Instance.CellToWorldPosition(newValue);
        }
    }
}