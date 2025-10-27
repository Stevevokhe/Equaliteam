using UnityEngine;

public class LightBulbScrewChecker : MonoBehaviour
{
    FaultyLightBulbMinigameManager manager;
    [SerializeField] private int stageToActivate;    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        manager = GameObject.FindAnyObjectByType<FaultyLightBulbMinigameManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.transform.CompareTag("LightBulbPuzzle"))
        {
            manager.ActivateStage(stageToActivate);
        }
    }
}
