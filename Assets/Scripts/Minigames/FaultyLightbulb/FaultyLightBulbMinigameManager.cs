using UnityEngine;


public class FaultyLightBulbMinigameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] stages;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Stage1();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Stage1()
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

    public void Stage2()
    {

    }
    public void Stage3()
    {

    }
    public void Stage4()
    {

    }
    public void Stage5()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("LightBulbPuzzle"))
        {
            collision.gameObject.GetComponent<FaultyLightBulbController>().ResetPosition();
        }
    }
    
  
}
