using System;
using UnityEngine;

public class EventBus
{
    public static Action<string> OnMinigameCalled;
    public static Action OnMinigameCompleted;

    public static void InvokeOnMinigameCalled(string minigameName) => OnMinigameCalled?.Invoke(minigameName);
    public static void InvokeOnMinigameCompleted() => OnMinigameCompleted?.Invoke();
}
