using UnityEngine;
using UnityEngine.EventSystems;

public class MrWhiskers : Minigame
{
    [Header("UI Object")]
    [SerializeField] private GameObject whiskersObject; // The UI object that will move

    [Header("Target Points")]
    [SerializeField] private Transform whiskersTargetPoint;
    [SerializeField] private Transform playerTargetPoint;

    [Header("Movement Settings")]
    [SerializeField] private float naturalMoveSpeed = 2f;
    [SerializeField] private float playerPushForce = 5f;
    [SerializeField] private float dragMultiplier = 3f;
    [SerializeField] private float arrivalThreshold = 0.1f;

    [Header("State")]
    [SerializeField] private bool isActive = false;

    private RectTransform rectTransform;
    private Canvas canvas;
    private EventTrigger eventTrigger;
    private bool isBeingDragged = false;
    private bool hasReachedDestination = false;
    private float currentPushTimer = 0f;
    private float pushDuration = 0.2f;
    private Vector3 initialPosition;

    private void Awake()
    {
        if (whiskersObject == null)
        {
            Debug.LogError("MrWhiskers: No UI object assigned to control!");
            return;
        }

        rectTransform = whiskersObject.GetComponent<RectTransform>();
        canvas = whiskersObject.GetComponentInParent<Canvas>();

        initialPosition = rectTransform.position;

        SetupEventTrigger();
    }

    private void SetupEventTrigger()
    {
        eventTrigger = whiskersObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = whiskersObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        eventTrigger.triggers.Add(pointerDownEntry);

        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        eventTrigger.triggers.Add(dragEntry);

        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        eventTrigger.triggers.Add(pointerUpEntry);
    }

    public override void StartMinigame()
    {
        EventBus.InvokeOnSFXCalled(SFXType.CatCrowl);
        isActive = true;
        hasReachedDestination = false;
        isBeingDragged = false;
        currentPushTimer = 0f;
    }

    public override void StopMinigame()
    {
        isActive = false;
        isBeingDragged = false;
    }

    private void Update()
    {
        if (!isActive || hasReachedDestination || rectTransform == null)
            return;

        if (!isBeingDragged && currentPushTimer <= 0f)
        {
            MoveTowardTarget(whiskersTargetPoint.position, naturalMoveSpeed);
        }
        else if (currentPushTimer > 0f)
        {
            MoveTowardTarget(playerTargetPoint.position, playerPushForce);
            currentPushTimer -= Time.deltaTime;
        }

        if (Vector3.Distance(rectTransform.position, playerTargetPoint.position) < arrivalThreshold)
        {
            hasReachedDestination = true;
            rectTransform.position = playerTargetPoint.position;
            OnPlayerWins();
        }
    }

    private void MoveTowardTarget(Vector3 targetPosition, float speed)
    {
        rectTransform.position = Vector3.MoveTowards(
            rectTransform.position,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    private void OnPointerDown(PointerEventData eventData)
    {
        if (!isActive || hasReachedDestination)
            return;

        currentPushTimer = pushDuration;
    }

    private void OnDrag(PointerEventData eventData)
    {
        if (!isActive || hasReachedDestination)
            return;
        EventBus.InvokeOnSFXCalled(SFXType.CatPurr);
        isBeingDragged = true;
        MoveTowardTarget(playerTargetPoint.position, playerPushForce * dragMultiplier * Time.deltaTime);
    }

    private void OnPointerUp(PointerEventData eventData)
    {
        isBeingDragged = false;
    }

    private void OnReachedEnemyTarget()
    {
        Debug.Log("Object reached enemy target - Player Lost!");
        StopMinigame();
    }

    private void OnPlayerWins()
    {
        EventBus.InvokeOnSFXCalled(SFXType.CatMeow2);
        Debug.Log("Player pushed object to target - Player Wins!");
        EventBus.OnMinigameCompleted();

        // Reset position before deactivating
        rectTransform.position = initialPosition;

        gameObject.SetActive(false);
        StopMinigame();
    }

    private void OnDrawGizmos()
    {
        if (whiskersTargetPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(whiskersTargetPoint.position, 10f);
        }

        if (playerTargetPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerTargetPoint.position, 10f);
        }
    }
}