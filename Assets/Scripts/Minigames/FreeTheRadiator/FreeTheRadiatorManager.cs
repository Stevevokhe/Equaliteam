using UnityEngine;

public class FreeTheRadiatorManager : Minigame
{
    [Header("State")]
    [SerializeField] private bool isActive = false;
    [SerializeField] private int clothNumber;
    private int clothsCleared;
    public override void StartMinigame()
    {
        isActive = true;
    }

    public void ClothCleared()
    {
        clothsCleared++;
        if (clothsCleared >= clothNumber)
        {
            //TODO do effects & shit
            StopMinigame();
        }
    }

    public override void StopMinigame()
    {
        isActive=false;
        //EventBus.OnMinigameCompleted();
        gameObject.SetActive(false);
        
    }
}
