using UnityEngine;
using UnityEngine.UI;


public class FaultyLightBulbMinigameManager : Minigame
{
    [SerializeField] private GameObject[] stages;

    [SerializeField] private Image switchImage;
    [SerializeField] private Sprite switchOnSprite;
    [SerializeField] private Sprite switchOffSprite;

    [Header("State")]
    [SerializeField] private bool isActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       ActivateStage(0);
    }

    // Update is called once per frame
    void Update()
    {

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
    
    
  
}
