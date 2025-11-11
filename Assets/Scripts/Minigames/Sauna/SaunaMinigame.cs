using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SaunaMinigame : Minigame
{
    [Header("Draggable Objects")]
    [SerializeField] private List<GameObject> draggableObjects = new List<GameObject>();

    [Header("Settings")]
    [SerializeField] private float yThreshold = 200f; // Y position objects need to be dragged past (in screen space)
    [SerializeField] private float flyOffSpeed = 500f; // Speed objects fly off the top
    [SerializeField] private float outOfBoundsY = 100f; // How far off screen before considered out of bounds
    [SerializeField] private float returnSpeed = 10f; // Speed objects return to original position

    [Header("Debug Visualization")]
    [SerializeField] private bool showThresholdLine = true;
    [SerializeField] private Color thresholdLineColor = Color.yellow;

    [Header("State")]
    [SerializeField] private bool isActive = false;

    private Dictionary<GameObject, RectTransform> objectRectTransforms = new Dictionary<GameObject, RectTransform>();
    private Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, float> previousScreenY = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, bool> objectsCleared = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> objectsFlyingOff = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> objectsOutOfBounds = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> objectsReturning = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, Vector2> dragOffsets = new Dictionary<GameObject, Vector2>();
    private GameObject currentlyDraggedObject = null;
    private Canvas canvas;
    private Camera uiCamera;

    private void Awake()
    {
        // Get the canvas from the first object
        if (draggableObjects.Count > 0 && draggableObjects[0] != null)
        {
            canvas = draggableObjects[0].GetComponentInParent<Canvas>();
            uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        }

        // Set up all draggable objects
        foreach (GameObject obj in draggableObjects)
        {
            if (obj == null) continue;

            RectTransform rect = obj.GetComponent<RectTransform>();
            if (rect != null)
            {
                objectRectTransforms[obj] = rect;
                initialPositions[obj] = rect.position;
                previousScreenY[obj] = GetScreenY(rect);
                objectsCleared[obj] = false;
                objectsFlyingOff[obj] = false;
                objectsOutOfBounds[obj] = false;
                objectsReturning[obj] = false;
                dragOffsets[obj] = Vector2.zero;

                // Set up EventTrigger for this object
                SetupEventTrigger(obj);
            }
        }
    }

    // Helper method to get the screen Y position of a RectTransform
    private float GetScreenY(RectTransform rect)
    {
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, rect.position);
        return screenPos.y;
    }

    private void SetupEventTrigger(GameObject obj)
    {
        EventTrigger eventTrigger = obj.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = obj.AddComponent<EventTrigger>();
        }

        // Clear existing triggers to avoid duplicates
        eventTrigger.triggers.Clear();

        // Add BeginDrag event
        EventTrigger.Entry beginDragEntry = new EventTrigger.Entry();
        beginDragEntry.eventID = EventTriggerType.BeginDrag;
        beginDragEntry.callback.AddListener((data) => { OnBeginDrag(obj, (PointerEventData)data); });
        eventTrigger.triggers.Add(beginDragEntry);

        // Add Drag event
        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener((data) => { OnDrag(obj, (PointerEventData)data); });
        eventTrigger.triggers.Add(dragEntry);

        // Add EndDrag event
        EventTrigger.Entry endDragEntry = new EventTrigger.Entry();
        endDragEntry.eventID = EventTriggerType.EndDrag;
        endDragEntry.callback.AddListener((data) => { OnEndDrag(obj, (PointerEventData)data); });
        eventTrigger.triggers.Add(endDragEntry);
    }

    public override void StartMinigame()
    {
        isActive = true;

        // Reset all objects to initial state
        foreach (GameObject obj in draggableObjects)
        {
            if (obj == null) continue;

            objectsCleared[obj] = false;
            objectsFlyingOff[obj] = false;
            objectsOutOfBounds[obj] = false;
            objectsReturning[obj] = false;

            if (objectRectTransforms.ContainsKey(obj) && initialPositions.ContainsKey(obj))
            {
                objectRectTransforms[obj].position = initialPositions[obj];
                previousScreenY[obj] = GetScreenY(objectRectTransforms[obj]);
            }

            obj.SetActive(true);
        }

        currentlyDraggedObject = null;
    }

    public override void StopMinigame()
    {
        isActive = false;
        currentlyDraggedObject = null;

        // Return all objects to their original positions
        foreach (GameObject obj in draggableObjects)
        {
            if (obj == null) continue;

            if (objectRectTransforms.ContainsKey(obj) && initialPositions.ContainsKey(obj))
            {
                objectRectTransforms[obj].position = initialPositions[obj];
                previousScreenY[obj] = GetScreenY(objectRectTransforms[obj]);
            }

            objectsReturning[obj] = false;
            objectsFlyingOff[obj] = false;
        }
    }

    private void Update()
    {
        if (!isActive) return;

        // Update flying off objects
        foreach (GameObject obj in draggableObjects)
        {
            if (obj == null) continue;

            if (objectsFlyingOff[obj] && objectRectTransforms.ContainsKey(obj))
            {
                RectTransform rect = objectRectTransforms[obj];
                rect.position += Vector3.up * flyOffSpeed * Time.deltaTime;

                // Check if out of bounds (using screen space)
                float screenY = GetScreenY(rect);
                if (screenY > Screen.height + outOfBoundsY)
                {
                    if (!objectsOutOfBounds[obj])
                    {
                        objectsOutOfBounds[obj] = true;
                        obj.SetActive(false);
                        Debug.Log($"{obj.name} is now out of bounds!");
                    }
                }
            }
            // Update returning objects
            else if (objectsReturning[obj] && objectRectTransforms.ContainsKey(obj))
            {
                RectTransform rect = objectRectTransforms[obj];
                Vector3 targetPosition = initialPositions[obj];

                // Smoothly move back to original position
                rect.position = Vector3.Lerp(rect.position, targetPosition, returnSpeed * Time.deltaTime);

                // Check if close enough to snap to final position
                if (Vector3.Distance(rect.position, targetPosition) < 0.5f)
                {
                    rect.position = targetPosition;
                    objectsReturning[obj] = false;
                    Debug.Log($"{obj.name} returned to original position");
                }
            }
        }

        // Check if all objects are cleared AND out of bounds
        CheckWinCondition();
    }

    private void OnBeginDrag(GameObject obj, PointerEventData eventData)
    {
        if (!isActive || objectsCleared[obj]) return;

        currentlyDraggedObject = obj;
        objectsReturning[obj] = false; // Stop returning if was in progress

        // Store the starting screen Y position for line-crossing detection
        if (objectRectTransforms.ContainsKey(obj))
        {
            previousScreenY[obj] = GetScreenY(objectRectTransforms[obj]);
        }

        // Calculate offset between mouse and object center
        RectTransform rect = objectRectTransforms[obj];
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            uiCamera,
            out localPoint
        );

        Vector2 objectLocalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            RectTransformUtility.WorldToScreenPoint(uiCamera, rect.position),
            uiCamera,
            out objectLocalPos
        );

        dragOffsets[obj] = objectLocalPos - localPoint;
    }

    private void OnDrag(GameObject obj, PointerEventData eventData)
    {
        if (!isActive || objectsCleared[obj] || currentlyDraggedObject != obj) return;

        RectTransform rect = objectRectTransforms[obj];

        // Convert pointer position to canvas local position
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            uiCamera,
            out localPoint))
        {
            // Apply the drag offset and set position
            rect.position = canvas.transform.TransformPoint(localPoint + dragOffsets[obj]);

            // Get current screen Y position
            float currentScreenY = GetScreenY(rect);

            // Robust line-crossing detection (in screen space)
            if (HasCrossedThreshold(previousScreenY[obj], currentScreenY))
            {
                // Take control away from player and start flying off
                objectsCleared[obj] = true;
                objectsFlyingOff[obj] = true;
                currentlyDraggedObject = null; // Release control
                Debug.Log($"{obj.name} crossed threshold at Y={currentScreenY:F1} (threshold={yThreshold}) - flying off!");
            }

            // Update previous screen Y position for next frame
            previousScreenY[obj] = currentScreenY;
        }
    }

    private void OnEndDrag(GameObject obj, PointerEventData eventData)
    {
        if (!isActive) return;

        // Only process if this object is still being dragged
        if (currentlyDraggedObject == obj)
        {
            currentlyDraggedObject = null;

            // If object was dropped without crossing threshold, return it to original position
            if (!objectsCleared[obj])
            {
                objectsReturning[obj] = true;
                Debug.Log($"{obj.name} dropped without crossing threshold - returning to original position");
            }
        }
    }

    // Robust line-crossing detection (in screen space coordinates)
    private bool HasCrossedThreshold(float previousY, float currentY)
    {
        // Check if the object crossed from below to above the threshold
        bool wasBelowThreshold = previousY < yThreshold;
        bool isAboveThreshold = currentY >= yThreshold;

        return wasBelowThreshold && isAboveThreshold;
    }

    private void CheckWinCondition()
    {
        // Check if all objects are cleared AND out of bounds
        bool allClearedAndGone = true;

        foreach (GameObject obj in draggableObjects)
        {
            if (obj == null) continue;

            // Object must be both cleared and out of bounds
            if (!objectsCleared[obj] || !objectsOutOfBounds[obj])
            {
                allClearedAndGone = false;
                break;
            }
        }

        if (allClearedAndGone && draggableObjects.Count > 0)
        {
            OnPlayerWins();
        }
    }

    private void OnPlayerWins()
    {
        if (!isActive) return; // Prevent multiple calls

        Debug.Log("All objects cleared and off screen - Player Wins!");
        EventBus.OnMinigameCompleted();

        // Reset all objects to initial positions
        foreach (GameObject obj in draggableObjects)
        {
            if (obj == null) continue;

            if (objectRectTransforms.ContainsKey(obj) && initialPositions.ContainsKey(obj))
            {
                objectRectTransforms[obj].position = initialPositions[obj];
                previousScreenY[obj] = GetScreenY(objectRectTransforms[obj]);
            }

            obj.SetActive(true); // Reactivate objects that were deactivated
        }

        gameObject.SetActive(false);
        StopMinigame();
    }

    // Enhanced gizmo visualization - now draws HORIZONTAL line
    private void OnDrawGizmos()
    {
        if (!showThresholdLine) return;

        Gizmos.color = thresholdLineColor;

        // Draw a thick horizontal line at the Y threshold
        // Make it visible across the entire screen width
        float screenWidth = Screen.width > 0 ? Screen.width : 1920f; // Fallback for editor
        Vector3 leftPoint = new Vector3(-200f, yThreshold, 0);
        Vector3 rightPoint = new Vector3(screenWidth + 200f, yThreshold, 0);

        // Draw multiple lines to make it thicker and more visible
        for (int i = -2; i <= 2; i++)
        {
            Vector3 offset = new Vector3(0, i * 0.5f, 0);
            Gizmos.DrawLine(leftPoint + offset, rightPoint + offset);
        }

        // Draw markers along the line
        for (float x = 0; x < screenWidth; x += 100f)
        {
            Vector3 markerCenter = new Vector3(x, yThreshold, 0);
            Gizmos.DrawWireSphere(markerCenter, 20f);
        }

        // Draw label at the center
#if UNITY_EDITOR
        UnityEditor.Handles.color = thresholdLineColor;
        UnityEditor.Handles.Label(
            new Vector3(screenWidth * 0.5f, yThreshold, 0),
            $"Threshold Line (Screen Space)\nY = {yThreshold}",
            new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = thresholdLineColor },
                fontSize = 14,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            }
        );
#endif
    }

    // Draw the threshold line in the Game view as well (for runtime debugging)
    private void OnGUI()
    {
        if (!showThresholdLine || !isActive) return;

        // Draw the line in screen space
        Color oldColor = GUI.color;
        GUI.color = thresholdLineColor;

        // Draw horizontal line
        Rect lineRect = new Rect(0, Screen.height - yThreshold - 2, Screen.width, 4);
        GUI.DrawTexture(lineRect, Texture2D.whiteTexture);

        // Draw label
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.normal.textColor = thresholdLineColor;
        labelStyle.fontSize = 16;
        labelStyle.fontStyle = FontStyle.Bold;
        GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height - yThreshold - 30, 100, 30), $"Y = {yThreshold:F0}", labelStyle);

        GUI.color = oldColor;
    }
}