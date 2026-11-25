using Unity.Netcode;
using UnityEngine;

namespace RuntimeGrid
{
    public class RuntimeGridInteractor : NetworkBehaviour
    {
        public NetworkVariable<Vector2Int> SelectedCell = new NetworkVariable<Vector2Int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [SerializeField] private GameObject selectionObjectPrefab;
        private Transform selectionObject;

        [SerializeField] private bool forceMobileInteraction;
        [SerializeField] private float interactionRange = 2.5f;
        public bool CanInteractWithCell() => CanInteractWithCell(SelectedCell.Value);
        public bool CanInteractWithCell(Vector2Int cellPosition)
        {
            if (!RuntimeGridInstance.Instance.HasRuntimeCell(cellPosition))
                return false;

            Vector3 worldPos = RuntimeGridInstance.Instance.CellToWorldPosition(cellPosition);
            return Vector3.Distance(transform.position, worldPos) <= interactionRange;
        }

        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
                return;

            selectionObject = Instantiate(selectionObjectPrefab).transform;

            SelectedCell.OnValueChanged += OnChangedCell;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
                return;

            SelectedCell.OnValueChanged -= OnChangedCell;
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

            Vector2Int currentSelected;
            if (Application.isMobilePlatform || Application.isConsolePlatform || forceMobileInteraction)
            {
                // Get cell on which the player stands
                currentSelected = RuntimeGridInstance.Instance.WorldToCellPosition(transform.position);
            }
            else
            {
                // Get mouse position
                Vector2 mousePosition = PlayerInputs.Instance.Inputs.Player.PointerPosition.ReadValue<Vector2>();

                // Get world point position
                Ray ray = cam.ScreenPointToRay(mousePosition);
                RuntimeGridInstance.Instance.GetGridPlane().Raycast(ray, out float distance);
                Vector3 worldPointPosition = ray.GetPoint(distance);

                // Calculate cell position
                currentSelected = RuntimeGridInstance.Instance.WorldToCellPosition(worldPointPosition);
            }

            if (currentSelected != SelectedCell.Value)
                SelectedCell.Value = currentSelected;
        }

        private void OnChangedCell(Vector2Int previousValue, Vector2Int newValue)
        {
            selectionObject.gameObject.SetActive(CanInteractWithCell(newValue));
            selectionObject.position = RuntimeGridInstance.Instance.CellToWorldPosition(newValue);
        }
    }
}