using UnityEngine;

public class FreeTheRadiatorManager : Minigame
{
    [Header("State")]
    [SerializeField] private bool isActive = false;
    [SerializeField] private int clothNumber;
    [SerializeField] private GameObject[] clothes;
    [SerializeField] private Transform[] clothPositions;
    private FreeTheRadiatorHand hand;
    private int clothsCleared;
    public override void StartMinigame()
    { 
        hand = GameObject.FindAnyObjectByType<FreeTheRadiatorHand>();
        hand.ResetHand();
        clothsCleared=0;
        isActive = true;
        for(int i =0; i < clothes.Length; i++)
        {
            clothes[i].transform.position = clothPositions[i].transform.position;
            clothes[i].GetComponent<FreeTheRadiatorCloth>().ResetCloth();
        }
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
        EventBus.InvokeOnSFXCalled(SFXType.SuccesfullPuzzleCompleted);
        isActive =false;
        EventBus.OnMinigameCompleted();
        gameObject.SetActive(false);
        
    }
}
