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
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Animator animator;

    private const string ANIM_PLAYER_IS_WALKING = "_isWalking";
    private const string ANIM_PLAYER_WALKING_SPEED = "_walkingSpeed";

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        PerformPlayerMeshRotationUpdate();

        PerformPlayerAnimationUpdate();
    }

    private void PerformPlayerMeshRotationUpdate()
    {
        if (movementDirection == Vector3.zero)
            return;

        rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.LookRotation(movementDirection, Vector3.up), Time.deltaTime * 360 * rotationSpeed);
    }

    private void PerformPlayerAnimationUpdate()
    {
        animator.SetBool(ANIM_PLAYER_IS_WALKING, movementDirection != Vector3.zero);
        animator.SetFloat(ANIM_PLAYER_WALKING_SPEED, rb.linearVelocity.magnitude / movementSpeed);
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
