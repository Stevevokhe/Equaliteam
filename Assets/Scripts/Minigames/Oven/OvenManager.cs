using UnityEngine;

public class OvenManager : MonoBehaviour
{
    [SerializeField] GameObject fixButton;
   
    public int knobsToTurnOff;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KnobCompleted()
    {
        knobsToTurnOff++;
        if( knobsToTurnOff > 4)
        {
            //TODO do effects & shit
            fixButton.SetActive(true);
        }
    }
}
