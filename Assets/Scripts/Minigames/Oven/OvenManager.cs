using UnityEngine;

public class OvenManager : Minigame
{
    [Header("Reference")]
    [SerializeField] GameObject fixButton;
    [Header("State")]
    [SerializeField] private bool isActive = false;

    public int knobsToTurnOff;
    public override void StartMinigame()
    {
        isActive = true;
    }

    public void KnobCompleted()
    {
        knobsToTurnOff++;
        if( knobsToTurnOff >= 4)
        {
            //TODO do effects & shit
            fixButton.SetActive(true);
        }
    }
}
