using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField] private GameObject stage1VFX;

    [SerializeField] private bool deactivateAtStage2; // does stage 1 vfx needs to be deactivated at stage 2?

    [SerializeField] private GameObject stage2VFX;

    [SerializeField] private bool requiresActivationThroughScript;

    //STAGE 1 VFX Activate
    public void ActivateStage1VFX()
    {
        if (stage1VFX != null)
        {
            stage1VFX.SetActive(true);
            if (requiresActivationThroughScript)
            {
                var leak = stage1VFX.GetComponent<LeakController>();
                if (leak != null) leak.PlayLeak();
                else Debug.LogWarning("LeakController missing on Stage1VFX.");
            }
        }
        else
        {
            Debug.LogWarning("Stage 1 VFX missing in VFX Manager");
        }
    }
    //STAGE 1 VFX Deactivate
    public void DeactivateStage1VFX(bool force)
    {

        if (stage1VFX != null)
        {
            if (force || deactivateAtStage2)
                stage1VFX.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Stage 1 VFX missing in VFX Manager");
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
            if (!requiresActivationThroughScript)
            {
                ActivateStage1VFX();
            }
            Debug.LogWarning("Stage 2 VFX missing in VFX Manager");
        }
    }
    //STAGE 2 VFX Deactivate
    public void DeactivateStage2VFX()
    {
        if (stage2VFX != null)
        {
            stage2VFX.SetActive(false);
            return;
        }

        if (requiresActivationThroughScript)
        {
            if (stage1VFX == null)
            {
                Debug.LogWarning("Stage 1 VFX missing in VFX Manager");
                Debug.LogWarning("Stage 2 VFX missing in VFX Manager");
                return;
            }

            var leak = stage1VFX.GetComponent<LeakController>();
            if (leak != null) leak.ResetState();
            else Debug.LogWarning("LeakController missing on Stage1VFX.");
        }

        Debug.LogWarning("Stage 2 VFX missing in VFX Manager");
    }


    //
    public void DeactivateAllVFX()
    {
        DeactivateStage1VFX(true);
        DeactivateStage2VFX();
    }
}
