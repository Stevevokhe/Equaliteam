using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class FlipTheFuse : Minigame
{
    [Header("Switch Objects")]
    [SerializeField] private List<GameObject> switchObjects = new List<GameObject>();

    [Header("Settings")]
    [SerializeField] private int numberOfSwitchesStartingOff = 3;
    [SerializeField] private bool randomizeOnStart = true;

    [Header("Visual Settings")]
    [SerializeField] private Color onColor = Color.green;
    [SerializeField] private Color offColor = Color.red;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private bool useSprites = false;

    [Header("State")]
    [SerializeField] private bool isActive = false;

    private Dictionary<GameObject, bool> switchStates = new Dictionary<GameObject, bool>();
    private Dictionary<GameObject, Image> switchImages = new Dictionary<GameObject, Image>();
    private Dictionary<GameObject, EventTrigger> switchEventTriggers = new Dictionary<GameObject, EventTrigger>();

    private void Awake()
    {
        InitializeSwitches();
    }

    private void InitializeSwitches()
    {
        switchStates.Clear();
        switchImages.Clear();
        switchEventTriggers.Clear();

        foreach (GameObject switchObj in switchObjects)
        {
            if (switchObj == null) continue;

            Image img = switchObj.GetComponent<Image>();
            if (img == null)
            {
                img = switchObj.AddComponent<Image>();
            }
            switchImages[switchObj] = img;

            switchStates[switchObj] = true;

            SetupEventTrigger(switchObj);
        }

        Debug.Log($"Initialized {switchStates.Count} switches");
    }

    private void SetupEventTrigger(GameObject switchObj)
    {
        EventTrigger eventTrigger = switchObj.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = switchObj.AddComponent<EventTrigger>();
        }

        switchEventTriggers[switchObj] = eventTrigger;

        eventTrigger.triggers.Clear();

        EventTrigger.Entry clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerClick;
        clickEntry.callback.AddListener((data) => { OnSwitchClicked(switchObj); });
        eventTrigger.triggers.Add(clickEntry);
    }

    public override void StartMinigame()
    {
        Debug.Log("=== Starting FlipTheFuse Minigame ===");

        isActive = true;
        EventBus.InvokeOnSFXCalled(SFXType.ElectricHum);
        if (switchStates.Count != switchObjects.Count)
        {
            Debug.Log("Reinitializing switches due to count mismatch");
            InitializeSwitches();
        }

        List<GameObject> validSwitches = new List<GameObject>();
        foreach (GameObject switchObj in switchObjects)
        {
            if (switchObj == null) continue;

            switchStates[switchObj] = true;
            validSwitches.Add(switchObj);
        }

        Debug.Log($"Reset {validSwitches.Count} switches to ON state");
        if (randomizeOnStart)
        {
            RandomizeSwitches();
        }

        UpdateAllSwitchVisuals();

        LogSwitchStates();
    }

    public override void StopMinigame()
    {
        Debug.Log("=== Stopping FlipTheFuse Minigame ===");

        isActive = false;

        foreach (GameObject switchObj in switchObjects)
        {
            if (switchObj == null) continue;
            if (switchStates.ContainsKey(switchObj))
            {
                switchStates[switchObj] = true;
            }
        }

        UpdateAllSwitchVisuals();
    }

    private void RandomizeSwitches()
    {
        int numToTurnOff = Mathf.Clamp(numberOfSwitchesStartingOff, 0, switchObjects.Count);

        Debug.Log($"Attempting to turn off {numToTurnOff} switches");

        if (numToTurnOff <= 0)
        {
            Debug.Log("No switches to turn off");
            return;
        }

        List<GameObject> availableSwitches = new List<GameObject>();
        foreach (GameObject switchObj in switchObjects)
        {
            if (switchObj != null && switchStates.ContainsKey(switchObj))
            {
                availableSwitches.Add(switchObj);
            }
        }

        Debug.Log($"Found {availableSwitches.Count} available switches");

        int actuallyTurnedOff = 0;
        for (int i = 0; i < numToTurnOff && availableSwitches.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableSwitches.Count);
            GameObject selectedSwitch = availableSwitches[randomIndex];

            switchStates[selectedSwitch] = false;
            actuallyTurnedOff++;

            Debug.Log($"Turned OFF switch: {selectedSwitch.name}");

            availableSwitches.RemoveAt(randomIndex);
        }

        Debug.Log($"Actually turned off {actuallyTurnedOff} switches");
    }

    private void OnSwitchClicked(GameObject switchObj)
    {
        if (!isActive)
        {
            Debug.Log("Switch clicked but minigame is not active");
            return;
        }

        if (!switchStates.ContainsKey(switchObj))
        {
            Debug.LogError($"Switch {switchObj.name} not found in switchStates dictionary!");
            return;
        }

        switchStates[switchObj] = !switchStates[switchObj];
        EventBus.InvokeOnSFXCalled(SFXType.FuseClick);

        UpdateSwitchVisual(switchObj);

        string state = switchStates[switchObj] ? "ON" : "OFF";
        Debug.Log($"{switchObj.name} toggled to {state}");

        LogSwitchStates();

        CheckWinCondition();
    }

    private void UpdateSwitchVisual(GameObject switchObj)
    {
        if (!switchImages.ContainsKey(switchObj))
        {
            Debug.LogWarning($"No image found for switch {switchObj.name}");
            return;
        }

        Image img = switchImages[switchObj];
        bool isOn = switchStates[switchObj];

        if (useSprites && onSprite != null && offSprite != null)
        {
            img.sprite = isOn ? onSprite : offSprite;
        }
        else
        {
            img.color = isOn ? onColor : offColor;
        }
    }

    private void UpdateAllSwitchVisuals()
    {
        foreach (GameObject switchObj in switchObjects)
        {
            if (switchObj == null) continue;
            UpdateSwitchVisual(switchObj);
        }
    }

    private void CheckWinCondition()
    {
        if (!isActive)
        {
            Debug.Log("CheckWinCondition called but minigame is not active");
            return;
        }

        int onCount = 0;
        int offCount = 0;
        int totalCount = 0;

        foreach (GameObject switchObj in switchObjects)
        {
            if (switchObj == null) continue;
            if (!switchStates.ContainsKey(switchObj)) continue;

            totalCount++;
            if (switchStates[switchObj])
            {
                onCount++;
            }
            else
            {
                offCount++;
            }
        }

        Debug.Log($"Win Check - ON: {onCount}/{totalCount}, OFF: {offCount}/{totalCount}");

        bool allOn = (offCount == 0 && totalCount > 0);

        if (allOn)
        {
            Debug.Log("ALL SWITCHES ARE ON - WINNING!");
            OnPlayerWins();
        }
        else
        {
            Debug.Log($"Not all switches on yet. Still {offCount} switches OFF");
        }
    }

    private void OnPlayerWins()
    {
        if (!isActive)
        {
            Debug.Log("OnPlayerWins called but minigame already inactive");
            return;
        }

        Debug.Log("=== PLAYER WINS! ===");

        isActive = false;

        EventBus.OnMinigameCompleted();

        foreach (GameObject switchObj in switchObjects)
        {
            if (switchObj == null) continue;
            if (switchStates.ContainsKey(switchObj))
            {
                switchStates[switchObj] = true;
            }
        }

        UpdateAllSwitchVisuals();

        StopMinigame();

        gameObject.SetActive(false);
    }

    private void LogSwitchStates()
    {
        string stateLog = "Switch States: ";
        int onCount = 0;
        int offCount = 0;

        foreach (var kvp in switchStates)
        {
            if (kvp.Key == null) continue;

            string state = kvp.Value ? "ON" : "OFF";
            stateLog += $"{kvp.Key.name}={state}, ";

            if (kvp.Value) onCount++;
            else offCount++;
        }

        stateLog += $"[Total: ON={onCount}, OFF={offCount}]";
        Debug.Log(stateLog);
    }

    public void SetSwitchState(int index, bool isOn)
    {
        if (index < 0 || index >= switchObjects.Count) return;

        GameObject switchObj = switchObjects[index];
        if (switchObj == null) return;

        switchStates[switchObj] = isOn;
        UpdateSwitchVisual(switchObj);
    }

    public bool GetSwitchState(int index)
    {
        if (index < 0 || index >= switchObjects.Count) return false;

        GameObject switchObj = switchObjects[index];
        if (switchObj == null) return false;

        return switchStates[switchObj];
    }

    public int GetNumberOfSwitchesOn()
    {
        return switchStates.Count(kvp => kvp.Key != null && kvp.Value == true);
    }

    public int GetNumberOfSwitchesOff()
    {
        return switchStates.Count(kvp => kvp.Key != null && kvp.Value == false);
    }
}