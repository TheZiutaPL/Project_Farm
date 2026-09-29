using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : NetworkBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float movementAcceleration;
    [SerializeField] private float movementSpeed;
    private Vector3 movementDirection;

    [Header("Visuals")]
    [SerializeField] private Transform playerMesh;
    [SerializeField] private float rotationSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        PerformPlayerMeshRotationUpdate();
    }

    private void PerformPlayerMeshRotationUpdate()
    {
        if (movementDirection == Vector3.zero)
            return;

        playerMesh.rotation = Quaternion.RotateTowards(playerMesh.rotation, Quaternion.LookRotation(movementDirection, Vector3.up), Time.deltaTime * 360 * rotationSpeed);
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        PerformMovementFUpdate();

        PerformSpeedCapFUpdate();
    }

    private void PerformMovementFUpdate()
    {
        if (movementDirection == Vector3.zero)
            return;

        rb.AddForce(movementDirection * movementAcceleration);
    }

    private void PerformSpeedCapFUpdate()
    {
        if (rb.linearVelocity.magnitude > movementSpeed)
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, movementSpeed);
    }

    public void OnMoveInput_Performed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();

        movementDirection = new Vector3(input.x, 0, input.y).normalized;
    }
}
