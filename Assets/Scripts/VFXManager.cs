using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [Header("STAGE 2")][SerializeField] private GameObject stage2VFX;
    public AudioClip soundStage2;

    [SerializeField] private bool deactivateAtStage3; // does stage 1 vfx needs to be deactivated at stage 2?

    [Header("STAGE 3")][SerializeField] private GameObject stage3VFX;
    public AudioClip soundStage3;

    [Header("Enable ONLY for the leak")]
    [SerializeField] private bool requiresActivationThroughScript;



    //STAGE 2 VFX Activate
    public void ActivateStage2VFX()
    {
        if (stage2VFX != null)
        {
            stage2VFX.SetActive(true);

            if (soundStage2 != null)
            {
                // Activate sound stage 2
            }

            if (requiresActivationThroughScript) // If its the leak effect activate it
            {
                var leak = stage2VFX.GetComponent<LeakController>();
                if (leak != null) leak.PlayLeak();
                else Debug.LogWarning("LeakController missing on Stage2VFX");
            }
        }
        else
        {
            Debug.LogWarning("Stage 2 VFX missing in VFX Manager");
        }
    }
    //STAGE 2 VFX Deactivate
    public void DeactivateStage2VFX(bool force)
    {

        if (stage2VFX != null)
        {
            if (force || deactivateAtStage3)
                stage2VFX.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Stage 2 VFX missing in VFX Manager");
        }
    }

    //=======================================================================

    //STAGE 3 VFX Activate
    public void ActivateStage3VFX()
    {
        if (stage3VFX != null)
        {
            stage3VFX.SetActive(true);

            if (soundStage3 != null)
            {
                // Activate sound stage 3
            }
        }
        else // if the stage 3 effect is null - keep stage 2 effect
        {
            if (!requiresActivationThroughScript)
            {
                ActivateStage2VFX();
            }
            Debug.LogWarning("Stage 3 VFX missing in VFX Manager");
        }

    }
    //STAGE 2 VFX Deactivate
    public void DeactivateStage3VFX()
    {
        if (stage3VFX != null)
        {
            stage3VFX.SetActive(false);
            return;
        }

        if (requiresActivationThroughScript)
        {

            var leak = stage2VFX.GetComponent<LeakController>();
            if (leak != null) leak.ResetState();
            else Debug.LogWarning("LeakController missing on Stage2VFX");
        }

        Debug.LogWarning("Stage 3 VFX missing in VFX Manager");
    }


    //
    public void DeactivateAllVFX()
    {
        DeactivateStage2VFX(true);
        DeactivateStage3VFX();
    }
}
