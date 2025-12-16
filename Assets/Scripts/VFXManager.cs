using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private GameObject stage1VFX;

    [SerializeField] private bool deactgivateAtStage2; // does stage 1 vfx needs to be deactivated at stage 2?

    [SerializeField] private GameObject stage2VFX;

    [SerializeField] private bool reqiresActivationThroughScript;

    //STAGE 1 VFX Activate
    public void ActivateStage1VFX()
    {
        if (stage1VFX != null)
        {
            stage1VFX.SetActive(true);
            if (reqiresActivationThroughScript)
            {
                stage1VFX.GetComponent<LeakController>().PlayLeak();
            }
        }
        else
        {
            Debug.Log("Stage 1 VFX missing in VFX Manager");
        }
    }
    //STAGE 1 VFX Deactivate
    public void DeactivateStage1VFX()
    {

        if (stage1VFX != null)
        {
            if (deactgivateAtStage2) // only deactivate if it needs to be deactivated
            {
                stage1VFX.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Stage 1 VFX missing in VFX Manager");
        }
    }

    //=======================================================================

    //STAGE 2 VFX Activate
    public void ActivateStage2VFX()
    {
        if (stage2VFX != null)
        {
            stage2VFX.SetActive(true);
        }
        else
        {
            if(!reqiresActivationThroughScript)
            ActivateStage1VFX();
            Debug.Log("Stage 2 VFX missing in VFX Manager");
        }
    }
    //STAGE 2 VFX Deactivate
    public void DeactivateStage2VFX()
    {
        if (stage2VFX != null)
        {
                stage2VFX.SetActive(false);
        }
        else
        {
            if (reqiresActivationThroughScript)
            {
                stage1VFX.GetComponent<LeakController>().ResetState();
            }
            Debug.Log("Stage 2 VFX missing in VFX Manager");
        }

    }


    //
    public void DeactivateAllVFX()
    {
        deactgivateAtStage2 = true;
        DeactivateStage1VFX();
        DeactivateStage2VFX();
        deactgivateAtStage2 = false;
    }
}
