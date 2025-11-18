using UnityEngine;

public class LightBulbScrewChecker : MonoBehaviour
{
    FaultyLightBulbMinigameManager manager;
    [SerializeField] private Transform placeToSnapBulb;
    void Start()
    {
        manager = GameObject.FindAnyObjectByType<FaultyLightBulbMinigameManager>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("LightBulbPuzzle"))
        {
            manager.ActivateStage(3);
            collision.gameObject.GetComponent<FaultyLightBulbNewBulb>().DraggedToPosition(placeToSnapBulb);
        }
    }

}
