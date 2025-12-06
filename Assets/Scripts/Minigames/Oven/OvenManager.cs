using UnityEngine;

public class OvenManager : Minigame
{
    [Header("State")]
    [SerializeField] private bool isActive = false;

    public int knobsToTurnOff;
    public override void StartMinigame()
    {
        isActive = true;
        EventBus.InvokeOnKnobReseted();
    }

    public void KnobCompleted()
    {
        knobsToTurnOff++;
        if( knobsToTurnOff >= 4)
        {
            //TODO do effects & shit
            StopMinigame();
        }
    }

    public override void StopMinigame()
    {
        isActive = false;
        EventBus.OnMinigameCompleted();
        gameObject.SetActive(false);

    }
}
