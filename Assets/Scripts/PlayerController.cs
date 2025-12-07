using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private bool dashing;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float groundDrag;

    Camera cam;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;
  
    public bool canMove;

    private PlayerInput playerInput;
    private InputAction moveAction;

    private Vector3 moveDirection;
    private float horizontalInput, verticalInput, idleAnimationCounter;

    Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float idleAnimationTriggerTime;
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
        rb = GetComponentInChildren<Rigidbody>();
        rb.freezeRotation = true;
        currentTool = PlayerTool.None;
        animator.SetBool("IsCarrying", false);
        animator.SetBool("IsMoving", false);
    }


    private void FixedUpdate()
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
                animator.SetBool("IsMoving", false);
                rb.linearDamping = groundDrag * 2f;
            }
            else
            {
                animator.SetBool("IsMoving", true);
                rb.linearDamping = groundDrag;
            }
        }
        else
        {
            rb.linearDamping = 0;
        }

        //If we are not moving
        if (rb.linearVelocity.magnitude < 0.01f)
        {
            //And have nothing in our hands
            if (currentTool == PlayerTool.None) {
                idleAnimationCounter += Time.deltaTime;
            }
            //And the idle animation time has passed
            if (idleAnimationCounter >= idleAnimationTriggerTime)
            {
                //Play a random emotion animation 
                var animName = Random.value > 0.5f ? "IsSurprised" : "IsScared";
                animator.SetTrigger(animName);
                idleAnimationCounter = 0;
                //And add a delay until the next one
                idleAnimationCounter -= idleAnimationTriggerTime * 3;
            }
        } else
        {
            idleAnimationCounter = 0;
        }

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
        currentTool = CurrentToolSO.PlayerTool;
        animator.SetBool("IsCarrying", true);
        CarriedToolObject = Instantiate(CurrentToolSO.CarriedToolObject, playerToolTransform);
    }

    public void DropPlayerTool()
    {
        currentTool = PlayerTool.None;
        animator.SetBool("IsCarrying", false);
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
            rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Force);

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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