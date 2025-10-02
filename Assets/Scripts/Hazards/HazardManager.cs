using System;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager : MonoBehaviour
{
    public static HazardManager Instance;

    [Header("Hazard Management")]
    public List<HazardBase> Hazards;

    [Header("Debug")]
    public bool showDebugInfo = false;

    private List<Timer> activeTimers = new List<Timer>();
    private List<Timer> timersToRemove = new List<Timer>();

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
    }

    private void Update()
    {
        UpdateTimers();
    }

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

    public Timer CreateTimer(float duration, Action onComplete, bool loop = false)
    {
        Timer timer = new Timer(duration, onComplete, loop);
        activeTimers.Add(timer);
        return timer;
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
    }

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
        }
    }

    public void UnregisterHazard(HazardBase hazard)
    {
        if (Hazards.Count > 0 && Hazards.Contains(hazard))
        {
            Hazards.Remove(hazard);
        }
    }
}