using UnityEngine;

public class OvenManager : Minigame
{
    [Header("State")]
    [SerializeField] private bool isActive = false;
    [SerializeField] private OvenKnobController[] knobs;
    public int knobsToTurnOff;
    public override void StartMinigame()
    {
        isActive = true;
        knobsToTurnOff = 0;
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
        EventBus.InvokeOnSFXCalled(SFXType.SuccesfullPuzzleCompleted);
        isActive = false;
        EventBus.OnMinigameCompleted();
        gameObject.SetActive(false);

    }
}
