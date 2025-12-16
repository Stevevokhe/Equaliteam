using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FireplaceFlick : Minigame
{
    [Header("Objects")]
    [SerializeField] private List<GameObject> clickableObjects = new List<GameObject>();
    [SerializeField] private Transform targetObject;

    [Header("Settings")]
    [SerializeField] private float flipSpeed = 500f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float arrivalThreshold = 10f;
    [SerializeField] private float landingSpread = 50f;

    [Header("State")]
    [SerializeField] private bool isActive = false;

    private Dictionary<GameObject, RectTransform> objectRectTransforms = new Dictionary<GameObject, RectTransform>();
    private Dictionary<GameObject, Vector3> initialPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> initialRotations = new Dictionary<GameObject, Quaternion>();
    private Dictionary<GameObject, Vector3> targetPositions = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, bool> objectsClicked = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> objectsFlipping = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, bool> objectsArrived = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, EventTrigger> eventTriggers = new Dictionary<GameObject, EventTrigger>();

    private void Awake()
    {
        foreach (GameObject obj in clickableObjects)
        {
            if (obj == null) continue;

            RectTransform rect = obj.GetComponent<RectTransform>();
            if (rect != null)
            {
                objectRectTransforms[obj] = rect;
                initialPositions[obj] = rect.position;
                initialRotations[obj] = rect.rotation;
                objectsClicked[obj] = false;
                objectsFlipping[obj] = false;
                objectsArrived[obj] = false;

                SetupEventTrigger(obj);
            }
        }
    }

    private void SetupEventTrigger(GameObject obj)
    {
        EventTrigger eventTrigger = obj.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = obj.AddComponent<EventTrigger>();
        }

        eventTriggers[obj] = eventTrigger;
        eventTrigger.triggers.Clear();

        EventTrigger.Entry clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerClick;
        clickEntry.callback.AddListener((data) => { OnObjectClicked(obj); });
        eventTrigger.triggers.Add(clickEntry);
    }

    public override void StartMinigame()
    {
        EventBus.InvokeOnSFXCalled(SFXType.FrameStartBurn);

        isActive = true;

        foreach (GameObject obj in clickableObjects)
        {
            if (obj == null) continue;

            objectsClicked[obj] = false;
            objectsFlipping[obj] = false;
            objectsArrived[obj] = false;

            if (objectRectTransforms.ContainsKey(obj) && initialPositions.ContainsKey(obj))
            {
                objectRectTransforms[obj].position = initialPositions[obj];
                objectRectTransforms[obj].rotation = initialRotations[obj];
            }

            obj.SetActive(true);
        }

        Debug.Log("SaunaClick&Flick minigame started");
    }

    public override void StopMinigame()
    {
        isActive = false;

        foreach (GameObject obj in clickableObjects)
        {
            if (obj == null) continue;

            if (objectRectTransforms.ContainsKey(obj) && initialPositions.ContainsKey(obj))
            {
                objectRectTransforms[obj].position = initialPositions[obj];
                objectRectTransforms[obj].rotation = initialRotations[obj];
            }

            objectsFlipping[obj] = false;
        }

        Debug.Log("SaunaClick&Flick minigame stopped");
    }

    private void Update()
    {
        if (!isActive) return;

        foreach (GameObject obj in clickableObjects)
        {
            if (obj == null) continue;

            if (objectsFlipping[obj] && !objectsArrived[obj] && objectRectTransforms.ContainsKey(obj))
            {
                RectTransform rect = objectRectTransforms[obj];
                Vector3 targetPos = targetPositions[obj];

                rect.position = Vector3.MoveTowards(
                    rect.position,
                    targetPos,
                    flipSpeed * Time.deltaTime
                );

                rect.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

                if (Vector3.Distance(rect.position, targetPos) < arrivalThreshold)
                {
                    objectsArrived[obj] = true;
                    rect.position = targetPos;
                    Debug.Log($"{obj.name} arrived at target position");

                    CheckWinCondition();
                }
            }
        }
    }

    private void OnObjectClicked(GameObject obj)
    {
        if (!isActive) return;
        if (objectsClicked[obj]) return;

        EventBus.InvokeOnSFXCalled(SFXType.DraggingSomething);
        objectsClicked[obj] = true;
        objectsFlipping[obj] = true;

        Vector2 randomOffset = Random.insideUnitCircle * landingSpread;
        Vector3 randomizedTarget = targetObject.position + new Vector3(randomOffset.x, randomOffset.y, 0);
        targetPositions[obj] = randomizedTarget;

        Debug.Log($"{obj.name} clicked - starting flip to randomized position");
    }

    private void CheckWinCondition()
    {
        bool allArrived = true;

        foreach (GameObject obj in clickableObjects)
        {
            if (obj == null) continue;

            if (!objectsArrived[obj])
            {
                allArrived = false;
                break;
            }
        }

        if (allArrived && clickableObjects.Count > 0)
        {
            OnPlayerWins();
        }
    }

    private void OnPlayerWins()
    {
        if (!isActive) return;

        Debug.Log("All objects flipped to target - Player Wins!");
        EventBus.OnMinigameCompleted();

        foreach (GameObject obj in clickableObjects)
        {
            if (obj == null) continue;

            if (objectRectTransforms.ContainsKey(obj) && initialPositions.ContainsKey(obj))
            {
                objectRectTransforms[obj].position = initialPositions[obj];
                objectRectTransforms[obj].rotation = initialRotations[obj];
            }
        }

        StopMinigame();
        gameObject.SetActive(false);
    }
}