using UnityEngine;
using UnityEngine.UI;


public class FaultyLightBulbMinigameManager : Minigame
{
    [SerializeField] private GameObject[] stages;
    [SerializeField] private FaultyLightBulbController bulb1, bulb2;
    [SerializeField] private Image switchImage;
    [SerializeField] private Sprite switchOnSprite;
    [SerializeField] private Sprite switchOffSprite;

    [Header("State")]
    [SerializeField] private bool isActive = false;

    private GameObject bulbStartSnapPoint, bulbToPick;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       ActivateStage(0);
        bulbStartSnapPoint = GameObject.Find("BulbStartSnapPoint");
        bulbToPick = GameObject.Find("BulbToPick");
    }

    public override void StartMinigame()
    {
        isActive = true;
        ActivateStage(0);
        bulb1.ResetBulb();
        bulb2.ResetBulb();
        bulbToPick.transform.position = bulbStartSnapPoint.transform.position;
    }

    public void ResetPuzzle()
    {
        switchImage.sprite = switchOnSprite;
    }

    public void ActivateStage(int stageNumber)
    {
        foreach (var stage in stages) {
            if(stage != stages[stageNumber].gameObject)
            {
                stage.gameObject.SetActive(false);
            } else
            {
                stage.gameObject.SetActive(true);
            }
        }        
    }

    public void Stage1()
    {
        switchImage.sprite = switchOffSprite;
        ActivateStage(1);
    }


    public void Stage5()
    {
        switchImage.sprite = switchOnSprite;
        isActive = false;

        EventBus.OnMinigameCompleted();
        StopMinigame();

        gameObject.SetActive(false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("LightBulbPuzzle"))
        {
            collision.gameObject.GetComponent<FaultyLightBulbController>().ResetPosition();
        }
    }

    public override void StopMinigame()
    {
        isActive = false;
        EventBus.OnMinigameCompleted();
        gameObject.SetActive(false);

    }

}
