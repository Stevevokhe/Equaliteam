using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
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

    [SerializeField] private HazardBase interactedHazard;

    [Header("Tools")]
    private PlayerTool currentTool;
    public Transform playerToolTransform;
    public PlayerToolSO CurrentToolSO;
    public GameObject CarriedToolObject;

    private bool inRangeOfHazard = false;

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
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        if (UnityEngine.Input.GetKeyUp(KeyCode.E) && !inRangeOfHazard && CarriedToolObject != null)
        {
            DropPlayerTool();
        }

        if (canMove)
        {
            Vector2 input = moveAction.ReadValue<Vector2>();
            horizontalInput = input.x;
            verticalInput = input.y;
        }

        if (grounded)
        {
            if (Mathf.Abs(horizontalInput) < 0.1f && Mathf.Abs(verticalInput) < 0.1f)
            {
                rb.linearDamping = groundDrag * 2f;
            }
            else
            {
                rb.linearDamping = groundDrag;
            }
        }
        else
        {
            rb.linearDamping = 0;
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

    public void SetHazardRangeBool(bool value)
    {
        inRangeOfHazard = value;
    }

    public void SetPlayerTool(PlayerTool tool)
    {
        currentTool = tool;
    }

    public void PickUpPlayerTool(PlayerToolSO toolSO)
    {
        CurrentToolSO = toolSO;
        CarriedToolObject = Instantiate(CurrentToolSO.CarriedToolObject, playerToolTransform);
    }

    public void DropPlayerTool()
    {
        Destroy(CarriedToolObject);
        Instantiate(CurrentToolSO.InteractableToolObject, transform.position, Quaternion.identity);
    }

    public PlayerTool GetCurrentTool()
    {
        return currentTool;
    }

    public void SetInteractedHazard(HazardBase hazard)
    {
        interactedHazard = hazard;
    }

    public void ResetInteractionHazard()
    {
        interactedHazard = null;
    }

    public void FixHazard()
    {
        SetCanMoveBool(true);
        interactedHazard.ResolveHazard();
    }

    public void SetCanMoveBool(bool set)
    {
        canMove = set;
    }

    private void MovePlayer()
    {
        Vector3 cameraForward = cam.transform.forward;
        Vector3 cameraRight = cam.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection = cameraForward * verticalInput + cameraRight * horizontalInput;

        if (moveDirection.magnitude > 0.1f)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
    }

    private void OnEnable()
    {
        EventBus.OnMinigameCompleted += FixHazard;
    }

    private void OnDisable()
    {
        EventBus.OnMinigameCompleted -= FixHazard;
    }

}