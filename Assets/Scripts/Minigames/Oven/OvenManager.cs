using UnityEngine;

public class OvenManager : MonoBehaviour
{
    [SerializeField] GameObject fixButton;
   
    public int knobsToTurnOff;
    

    public void KnobCompleted()
    {
        knobsToTurnOff++;
        if( knobsToTurnOff >= 4)
        {
            //TODO do effects & shit
            fixButton.SetActive(true);
        }
    }
}
