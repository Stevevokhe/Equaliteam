using System;
using UnityEngine;

public class Timer
{
    public float Duration { get; private set; }
    public float CurrentTime { get; set; }
    public Action OnComplete { get; set; }
    public bool IsLooping { get; set; }
    public bool IsPaused { get; set; }

    public float RemainingTime => Mathf.Max(0, Duration - CurrentTime);
    public float Progress => Mathf.Clamp01(CurrentTime / Duration);

    public Timer(float duration, Action onComplete, bool loop = false)
    {
        Duration = duration;
        OnComplete = onComplete;
        IsLooping = loop;
        CurrentTime = 0f;
        IsPaused = false;
    }

    public void Pause() => IsPaused = true;
    public void Resume() => IsPaused = false;
    public void Reset() => CurrentTime = 0f;
    public void Cancel() => HazardManager.Instance.RemoveTimer(this);
}