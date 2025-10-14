using System;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager : MonoBehaviour
{
    public static HazardManager Instance;

    [Header("Hazard Management")]
    public List<HazardBase> Hazards;

    [Header("Hazard Triggering")]
    [Tooltip("Minimum seconds between hazard triggers")]
    public float minHazardInterval = 4f;

    [Tooltip("Maximum seconds between hazard triggers")]
    public float maxHazardInterval = 6f;

    [Tooltip("How many hazards can be active at once")]
    public int maxActiveHazards = 3;

    [Header("Timer Management")]
    [Tooltip("Maximum number of timers that can be active at once. Set to 0 for unlimited.")]
    public int maxActiveTimers = 10;

    [Tooltip("What to do when max timers is reached")]
    public TimerLimitBehavior limitBehavior = TimerLimitBehavior.Queue;

    [Header("Debug")]
    public bool showDebugInfo = false;

    private List<Timer> activeTimers = new List<Timer>();
    private List<Timer> timersToRemove = new List<Timer>();
    private Queue<Timer> queuedTimers = new Queue<Timer>();
    private Timer hazardCheckTimer;

    public enum TimerLimitBehavior
    {
        Queue,
        Reject,
        ReplaceOldest,
        ReplaceShortest
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        HazardBase[] sceneHazards = FindObjectsByType<HazardBase>(FindObjectsSortMode.None);
        Hazards = new List<HazardBase>(sceneHazards);
        Debug.Log($"HazardManager initialized with {Hazards.Count} hazards");

        // Initialize hazards with random delays up to maxActiveHazards
        InitializeHazardsWithRandomDelays();
    }

    private void InitializeHazardsWithRandomDelays()
    {
        // Trigger hazards up to the max limit with random delays
        int hazardsToInitialize = Mathf.Min(maxActiveHazards, Hazards.Count);

        for (int i = 0; i < hazardsToInitialize; i++)
        {
            float randomDelay = GetRandomInterval();

            if (showDebugInfo)
            {
                Debug.Log($"Scheduling initial hazard trigger in {randomDelay:F2} seconds");
            }

            CreateTimer(randomDelay, CheckAndTriggerHazards, loop: false);
        }
    }

    private float GetRandomInterval()
    {
        return UnityEngine.Random.Range(minHazardInterval, maxHazardInterval);
    }

    private void Update()
    {
        UpdateTimers();
        CheckActiveTimers();
        ProcessQueuedTimers();
        UpdateAllHazards();
    }

    private void UpdateAllHazards()
    {
        // Update all active hazards
        float deltaTime = Time.deltaTime;
        for (int i = Hazards.Count - 1; i >= 0; i--)
        {
            if (Hazards[i] != null && Hazards[i].IsActive)
            {
                Hazards[i].UpdateHazard(deltaTime);
            }
        }
    }

    private void CheckAndTriggerHazards()
    {
        // Count how many hazards are currently active
        int activeHazardCount = GetActiveHazardCount();

        if (showDebugInfo)
        {
            Debug.Log($"Active hazards: {activeHazardCount}/{maxActiveHazards}");
        }

        // If we can trigger more hazards
        if (activeHazardCount < maxActiveHazards)
        {
            // Find inactive hazards
            List<HazardBase> inactiveHazards = GetInactiveHazards();

            if (inactiveHazards.Count > 0)
            {
                // Randomly select one to trigger
                HazardBase hazardToTrigger = inactiveHazards[UnityEngine.Random.Range(0, inactiveHazards.Count)];
                TriggerHazard(hazardToTrigger);
            }

            // Schedule the next check with a random interval
            ScheduleNextHazardCheck();
        }
        else
        {
            // If at max capacity, schedule a check for later
            ScheduleNextHazardCheck();
        }
    }

    private void ScheduleNextHazardCheck()
    {
        float randomInterval = GetRandomInterval();

        if (showDebugInfo)
        {
            Debug.Log($"Next hazard check scheduled in {randomInterval:F2} seconds");
        }

        CreateTimer(randomInterval, CheckAndTriggerHazards, loop: false);
    }

    public void TriggerHazard(HazardBase hazard)
    {
        if (hazard != null && !hazard.IsActive)
        {
            hazard.ActivateHazard();

            if (showDebugInfo)
            {
                Debug.Log($"Triggered hazard: {hazard.hazardName}");
            }
        }
    }

    private int GetActiveHazardCount()
    {
        int count = 0;
        foreach (HazardBase hazard in Hazards)
        {
            if (hazard != null && hazard.IsActive)
            {
                count++;
            }
        }
        return count;
    }

    private List<HazardBase> GetInactiveHazards()
    {
        List<HazardBase> inactive = new List<HazardBase>();
        foreach (HazardBase hazard in Hazards)
        {
            if (hazard != null && !hazard.IsActive)
            {
                inactive.Add(hazard);
            }
        }
        return inactive;
    }

    // Timer Management Methods
    private void UpdateTimers()
    {
        float deltaTime = Time.deltaTime;

        for (int i = 0; i < activeTimers.Count; i++)
        {
            Timer timer = activeTimers[i];
            if (!timer.IsPaused)
            {
                timer.CurrentTime += deltaTime;

                if (timer.CurrentTime >= timer.Duration)
                {
                    timer.OnComplete?.Invoke();

                    if (timer.IsLooping)
                    {
                        timer.CurrentTime = 0f;
                    }
                    else
                    {
                        timersToRemove.Add(timer);
                    }
                }
            }
        }

        if (timersToRemove.Count > 0)
        {
            foreach (Timer timer in timersToRemove)
            {
                activeTimers.Remove(timer);
            }
            timersToRemove.Clear();
        }
    }

    public void CheckActiveTimers()
    {
        if (showDebugInfo && activeTimers.Count > 0)
        {
            if (Time.frameCount % 60 == 0)
            {
                Debug.Log($"Active Timers: {activeTimers.Count}/{(maxActiveTimers == 0 ? "Unlimited" : maxActiveTimers.ToString())} | Queued: {queuedTimers.Count}");
            }
        }
    }

    private void ProcessQueuedTimers()
    {
        while (queuedTimers.Count > 0 && CanAddTimer())
        {
            Timer timer = queuedTimers.Dequeue();
            activeTimers.Add(timer);

            if (showDebugInfo)
            {
                Debug.Log($"Activated queued timer. Active: {activeTimers.Count}, Queued: {queuedTimers.Count}");
            }
        }
    }

    private bool CanAddTimer()
    {
        return maxActiveTimers == 0 || activeTimers.Count < maxActiveTimers;
    }

    public Timer CreateTimer(float duration, Action onComplete, bool loop = false)
    {
        Timer timer = new Timer(duration, onComplete, loop);

        if (CanAddTimer())
        {
            activeTimers.Add(timer);
            return timer;
        }

        switch (limitBehavior)
        {
            case TimerLimitBehavior.Queue:
                queuedTimers.Enqueue(timer);
                if (showDebugInfo)
                {
                    Debug.Log($"Timer queued. Queue size: {queuedTimers.Count}");
                }
                return timer;

            case TimerLimitBehavior.Reject:
                if (showDebugInfo)
                {
                    Debug.LogWarning($"Timer rejected. Max limit ({maxActiveTimers}) reached.");
                }
                return null;

            case TimerLimitBehavior.ReplaceOldest:
                if (activeTimers.Count > 0)
                {
                    Timer oldestTimer = activeTimers[0];
                    activeTimers.RemoveAt(0);
                    activeTimers.Add(timer);

                    if (showDebugInfo)
                    {
                        Debug.Log("Replaced oldest timer with new timer.");
                    }
                }
                return timer;

            case TimerLimitBehavior.ReplaceShortest:
                Timer shortestTimer = FindTimerWithShortestRemaining();
                if (shortestTimer != null)
                {
                    activeTimers.Remove(shortestTimer);
                    activeTimers.Add(timer);

                    if (showDebugInfo)
                    {
                        Debug.Log("Replaced timer with shortest remaining duration.");
                    }
                }
                return timer;

            default:
                return null;
        }
    }

    private Timer FindTimerWithShortestRemaining()
    {
        if (activeTimers.Count == 0) return null;

        Timer shortest = activeTimers[0];
        float shortestRemaining = shortest.Duration - shortest.CurrentTime;

        for (int i = 1; i < activeTimers.Count; i++)
        {
            float remaining = activeTimers[i].Duration - activeTimers[i].CurrentTime;
            if (remaining < shortestRemaining)
            {
                shortest = activeTimers[i];
                shortestRemaining = remaining;
            }
        }

        return shortest;
    }

    public void RemoveTimer(Timer timer)
    {
        if (activeTimers.Contains(timer))
        {
            activeTimers.Remove(timer);
        }
    }

    public void RemoveAllTimers()
    {
        activeTimers.Clear();
        timersToRemove.Clear();
        queuedTimers.Clear();
    }

    public void ClearQueuedTimers()
    {
        queuedTimers.Clear();
    }

    // Getter methods for monitoring
    public int GetActiveTimerCount()
    {
        return activeTimers.Count;
    }

    public int GetQueuedTimerCount()
    {
        return queuedTimers.Count;
    }

    public int GetMaxTimerLimit()
    {
        return maxActiveTimers;
    }

    public void SetMaxTimerLimit(int limit)
    {
        maxActiveTimers = Mathf.Max(0, limit);
    }

    // Hazard Control Methods
    public void PauseAllHazards()
    {
        foreach (HazardBase hazard in Hazards)
        {
            hazard.PauseHazard();
        }
    }

    public void ResumeAllHazards()
    {
        foreach (HazardBase hazard in Hazards)
        {
            hazard.ResumeHazard();
        }
    }

    public void ResetAllHazards()
    {
        foreach (HazardBase hazard in Hazards)
        {
            hazard.ResetHazardPhase();
        }
    }

    public int GetHazardCountByPhase(int phase)
    {
        int count = 0;
        foreach (HazardBase hazard in Hazards)
        {
            if (hazard.GetHazardPhase() == phase)
                count++;
        }
        return count;
    }

    public void RegisterHazard(HazardBase hazard)
    {
        if (!Hazards.Contains(hazard))
        {
            Hazards.Add(hazard);

            if (showDebugInfo)
            {
                Debug.Log($"Registered hazard: {hazard.hazardName}");
            }
        }
    }

    public void UnregisterHazard(HazardBase hazard)
    {
        if (Hazards.Count > 0 && Hazards.Contains(hazard))
        {
            Hazards.Remove(hazard);

            if (showDebugInfo)
            {
                Debug.Log($"Unregistered hazard: {hazard.hazardName}");
            }
        }
    }
}