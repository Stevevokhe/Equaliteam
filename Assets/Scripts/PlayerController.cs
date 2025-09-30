using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private bool dashing;

    public float groundDrag;

    Camera cam;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
  
    public bool canMove;

    private PlayerInput playerInput;
    private InputAction moveAction;

    private Vector3 moveDirection;
    private float horizontalInput, verticalInput;

    Rigidbody rb;
    private Animator animator;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        cam = FindAnyObjectByType<Camera>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
    }


    private void Update()
    {
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        if (canMove)
        {
            Vector2 input = moveAction.ReadValue<Vector2>();
            horizontalInput = input.x;
            verticalInput = input.y;
        }

        // Apply drag based on whether player is moving or not
        if (grounded)
        {
            // If no input, apply higher drag to slow down naturally
            if (Mathf.Abs(horizontalInput) < 0.1f && Mathf.Abs(verticalInput) < 0.1f)
            {
                rb.linearDamping = groundDrag * 2f; // Higher drag when not moving
            }
            else
            {
                rb.linearDamping = groundDrag; // Normal drag when moving
            }
        }
        else
        {
            rb.linearDamping = 0; // No drag in air
        }

        if (rb.linearVelocity.magnitude < 0.01f)
        {
            animator.SetBool("IsMoving", false);
        }
        else
        {
            animator.SetBool("IsMoving", true);
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            MovePlayer();
        }

    }

    public void EnablePlayerMovement()
    {
        canMove = true;
    }

    private void MovePlayer()
    {
        // Get camera forward and right directions projected onto the XZ plane (ground)
        Vector3 cameraForward = cam.transform.forward;
        Vector3 cameraRight = cam.transform.right;

        // Remove vertical component so movement is horizontal
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction based on camera orientation
        moveDirection = cameraForward * verticalInput + cameraRight * horizontalInput;

        // Only apply force if there's input
        if (moveDirection.magnitude > 0.1f)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
    }


}