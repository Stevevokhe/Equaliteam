using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MinigameManager : MonoBehaviour
{
    public List<MinigameUI> minigameUIList = new List<MinigameUI> ();
    private MinigameUI currentMinigameUI;
    private Minigame currentMinigame;

    [SerializeField] private GameObject interactableUI;

    public void CallForMinigameUI(string minigameToCall)
    {
        foreach (MinigameUI minigameUI in minigameUIList) 
        { 
            if (minigameUI.MinigameName == minigameToCall)
            {
                SetCurrentMinigameUI(minigameUI);
                break;
            }
        }
        Debug.Log("No Minigame with this name found!");
    }

    public void SetCurrentMinigameUI(MinigameUI currentUI)
    {
        currentMinigameUI = currentUI;
        currentMinigameUI.MinigameObject.SetActive(true);
        currentMinigame = currentMinigameUI.MinigameObject.GetComponent<Minigame>();
        currentMinigame.StartMinigame();
        
    }

    public void MinigameCompleted()
    {
        currentMinigameUI.MinigameObject.SetActive(false);

    }

    private void OnEnable()
    {
        EventBus.OnMinigameCalled += CallForMinigameUI;
        EventBus.OnMinigameCompleted += MinigameCompleted;
    }

    private void OnDisable()
    {
        EventBus.OnMinigameCalled -= CallForMinigameUI;
        EventBus.OnMinigameCompleted -= MinigameCompleted;
    }
}

[System.Serializable]
public class MinigameUI
{
    public string MinigameName;
    public GameObject MinigameObject;
}
