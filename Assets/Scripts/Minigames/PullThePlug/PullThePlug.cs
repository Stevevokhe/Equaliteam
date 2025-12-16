using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PullThePlug : Minigame
{
    [Header("UI Objects")]
    [SerializeField] private RectTransform ropeObject;

    [Header("Pull Settings")]
    [SerializeField] private float pullDistanceRequired = 200f;
    [SerializeField] private float snapBackSpeed = 8f;
    [SerializeField] private float maxStretchDistance = 250f;

    [Header("Drag Settings")]
    [SerializeField] private float dragSensitivity = 1f; // How much rope stretches per pixel of mouse movement
    [SerializeField] private float resistanceStrength = 0.5f; // Makes it harder to pull the further you go (0 = none, 1 = heavy)
    [SerializeField] private bool useResistance = true;

    [Header("Rope Stretch Settings")]
    [SerializeField] private float minRopeScaleY = 0.7f;

    [Header("State")]
    [SerializeField] private bool isActive = false;

    private Canvas canvas;
    private Camera canvasCamera;
    private EventTrigger eventTrigger;

    private bool isBeingDragged = false;
    private bool isUnplugged = false;

    // Initial state
    private Vector3 leftEdgeWorldPosition;
    private float initialWidth;
    private float initialHeight;
    private Vector3 initialScale;

    // Drag tracking
    private Vector3 dragStartMousePosition;
    private float stretchAtDragStart;

    private float currentPullDistance = 0f;

    private void Awake()
    {
        if (ropeObject == null)
        {
            Debug.LogError("PullThePlug: No rope object assigned!");
            return;
        }

        canvas = ropeObject.GetComponentInParent<Canvas>();

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            canvasCamera = canvas.worldCamera;
        }

        CaptureInitialStateAndSetPivot();
        SetupEventTrigger();
    }

    private void CaptureInitialStateAndSetPivot()
    {
        initialWidth = ropeObject.rect.width;
        initialHeight = ropeObject.rect.height;
        initialScale = ropeObject.localScale;

        Vector3[] corners = new Vector3[4];
        ropeObject.GetWorldCorners(corners);
        leftEdgeWorldPosition = (corners[0] + corners[1]) / 2f;

        ropeObject.pivot = new Vector2(0f, 0.5f);
        ropeObject.position = leftEdgeWorldPosition;
    }

    private void SetupEventTrigger()
    {
        eventTrigger = ropeObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = ropeObject.gameObject.AddComponent<EventTrigger>();
        }

        eventTrigger.triggers.Clear();

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
        isActive = true;
        isUnplugged = false;
        isBeingDragged = false;
        currentPullDistance = 0f;

        ResetToInitialState();
    }

    public override void StopMinigame()
    {
        isActive = false;
        isBeingDragged = false;
    }

    private void Update()
    {
        if (!isActive || isUnplugged || ropeObject == null)
            return;

        if (!isBeingDragged && currentPullDistance > 0f)
        {
            SnapBack();
        }

        UpdateRopeVisual();
    }

    private Vector3 GetMouseWorldPosition(PointerEventData eventData)
    {
        Vector3 mouseWorldPos;

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            mouseWorldPos = eventData.position;
        }
        else
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                ropeObject.parent as RectTransform,
                eventData.position,
                canvasCamera,
                out mouseWorldPos
            );
        }

        return mouseWorldPos;
    }

    private void OnPointerDown(PointerEventData eventData)
    {
        if (!isActive || isUnplugged)
            return;

        isBeingDragged = true;

        // Store where the drag started and current stretch amount
        dragStartMousePosition = GetMouseWorldPosition(eventData);
        stretchAtDragStart = currentPullDistance;
    }

    private void OnDrag(PointerEventData eventData)
    {
        if (!isActive || isUnplugged)
            return;

        isBeingDragged = true;

        Vector3 currentMousePos = GetMouseWorldPosition(eventData);

        // Calculate how far mouse moved from drag start (only horizontal)
        float mouseDelta = currentMousePos.x - dragStartMousePosition.x;

        // Apply sensitivity - this controls how much stretch per pixel of movement
        float pullAmount = stretchAtDragStart + (mouseDelta * dragSensitivity);

        // Only allow positive stretch
        pullAmount = Mathf.Max(0f, pullAmount);

        // Apply resistance if enabled
        if (useResistance && pullAmount > 0f)
        {
            float resistanceFactor = 1f - (resistanceStrength * (pullAmount / maxStretchDistance));
            resistanceFactor = Mathf.Max(0.2f, resistanceFactor);
            pullAmount *= resistanceFactor;
        }

        // Clamp to max stretch
        currentPullDistance = Mathf.Min(pullAmount, maxStretchDistance);

        if (currentPullDistance >= pullDistanceRequired)
        {
            OnPlugPulledOut();
        }
    }

    private void OnPointerUp(PointerEventData eventData)
    {
        isBeingDragged = false;
    }

    private void SnapBack()
    {
        currentPullDistance = Mathf.Lerp(currentPullDistance, 0f, snapBackSpeed * Time.deltaTime);

        if (currentPullDistance < 0.5f)
        {
            currentPullDistance = 0f;
        }
    }

    private void UpdateRopeVisual()
    {
        float newWidth = initialWidth + currentPullDistance;

        ropeObject.sizeDelta = new Vector2(newWidth, initialHeight);
        ropeObject.position = leftEdgeWorldPosition;

        float stretchRatio = currentPullDistance / pullDistanceRequired;
        float scaleY = Mathf.Lerp(initialScale.y, initialScale.y * minRopeScaleY, stretchRatio);
        ropeObject.localScale = new Vector3(initialScale.x, scaleY, initialScale.z);
    }

    private void ResetToInitialState()
    {
        if (ropeObject != null)
        {
            ropeObject.sizeDelta = new Vector2(initialWidth, initialHeight);
            ropeObject.position = leftEdgeWorldPosition;
            ropeObject.localScale = initialScale;
        }
    }

    private void OnPlugPulledOut()
    {
        Debug.Log("Plug pulled out - Player Wins!");
        isUnplugged = true;
        EventBus.InvokeOnSFXCalled(SFXType.DraggingSomething);
        EventBus.OnMinigameCompleted();

        ResetToInitialState();

        gameObject.SetActive(false);
        StopMinigame();
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(leftEdgeWorldPosition, 10f);
        }

        if (ropeObject != null)
        {
            Gizmos.color = Color.green;
            Vector3 targetPos = ropeObject.position + Vector3.right * (initialWidth + pullDistanceRequired);
            Gizmos.DrawWireSphere(targetPos, 10f);
        }
    }
}