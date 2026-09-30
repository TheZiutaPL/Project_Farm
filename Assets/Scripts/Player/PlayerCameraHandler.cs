using Unity.Netcode;
using UnityEngine;

public class PlayerCameraHandler : NetworkBehaviour
{
    [SerializeField] private Camera playerCameraPrefab;
    private Camera playerCameraReference;

    [Header("Position")]
    [SerializeField] private Vector3 cameraPointOffset = new Vector3(0, .5f, 0);
    [SerializeField] private Vector3 playerCameraRotation = new Vector3(45, 0, 0);
    [SerializeField] private float playerCameraDistance = 10f;

    [Header("Smoothing")]
    [SerializeField] private float smoothingTime = 0.1f;
    [SerializeField, Min(0.001f)] private float smoothingMargin = .05f;
    private Vector3 cameraVelocity;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            Destroy(this);
    }

    private void InstantiateCamera()
    {
        playerCameraReference = Instantiate(playerCameraPrefab, transform.position, Quaternion.Euler(playerCameraRotation));
    }

    private void FixedUpdate()
    {
        if (playerCameraReference == null)
            InstantiateCamera();

        Vector3 targetPosition = GetCameraTargetPosition();

        // Smoothing
        if(Vector3.Distance(playerCameraReference.transform.position, targetPosition) > smoothingMargin)
            targetPosition = Vector3.SmoothDamp(playerCameraReference.transform.position, targetPosition, ref cameraVelocity, smoothingTime, float.PositiveInfinity, Time.fixedDeltaTime);

        playerCameraReference.transform.position = targetPosition;
    }

    private Vector3 GetCameraTargetPosition() => transform.position + cameraPointOffset + -playerCameraReference.transform.forward * playerCameraDistance;
}
