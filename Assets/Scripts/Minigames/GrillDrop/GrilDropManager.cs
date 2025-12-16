using UnityEngine;

public class GrilDropManager : Minigame
{
    [Header("State")]
    [SerializeField] private bool isActive = false;
    private bool isTopConnected;
    private GrilDropController grillTop;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void StartMinigame()
    {
        grillTop = GameObject.FindAnyObjectByType<GrilDropController>();
        grillTop.ResetGrillTop();
        isActive = true;
        isTopConnected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isTopConnected)
            StopMinigame();
    }

    public void TopConnected()
    {
        isTopConnected = true;
    }

    public override void StopMinigame()
    {
        isActive = false;
        EventBus.OnMinigameCompleted();
        gameObject.SetActive(false);

    }
}
