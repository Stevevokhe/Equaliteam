using System;
using UnityEngine;

public class EventBus
{
    public static Action <string> OnMinigameCalled;
    public static Action OnMinigameCompleted;
    public static Action OnKnobReseted;
    public static Action OnFaultyLightBulbReseted;

    public static void InvokeOnMinigameCalled(string minigameName) => OnMinigameCalled?.Invoke(minigameName);
    public static void InvokeOnMinigameCompleted() => OnMinigameCompleted?.Invoke();
    public static void InvokeOnKnobReseted() => OnKnobReseted?.Invoke();
    public static void InvokeOnFaultyLightBulbReseted() => OnFaultyLightBulbReseted?.Invoke();
}
