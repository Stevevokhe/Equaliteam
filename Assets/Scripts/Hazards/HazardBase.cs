using UnityEngine;

public class HazardBase : MonoBehaviour
{
    public string hazardName;
    public int hazardPhase = 1;
    public PlayerTool requiredTool;
    public GameObject smokeObject;
    public float internalTimer;
    public float secondPhaseThreshold = 10f;
    public float thirdPhaseThreshold = 30f;

    private Timer hazardTimer;
    private bool isActive = false;

    private void Start()
    {
        SetVFX(false);
        InitializeHazard();
    }

    protected virtual void InitializeHazard()
    {
        hazardTimer = HazardManager.Instance.CreateTimer(0.1f, UpdateHazardTimer, loop: true);
        isActive = true;

        HazardManager.Instance.RegisterHazard(this);
    }

    private void UpdateHazardTimer()
    {
        if (!isActive) return;

        internalTimer += 0.1f;


        if (hazardPhase == 1 && internalTimer >= secondPhaseThreshold)
        {
            ProgressHazardPhase();
            TriggerPhase();
        }

        else if (hazardPhase == 2 && internalTimer >= thirdPhaseThreshold)
        {
            ProgressHazardPhase();
            TriggerPhase();
        }
    }

    public int GetHazardPhase()
    {
        return hazardPhase;
    }

    public bool CheckToolRequirement(PlayerTool heldTool)
    {
        return heldTool == requiredTool;
    }

    public void ProgressHazardPhase()
    {
        hazardPhase++;
        if (hazardPhase >= 3)
        {
            hazardPhase = 3;
        }
    }

    public void ResetHazardPhase()
    {
        hazardPhase = 1;
        internalTimer = 0f;
        TriggerPhase();
    }

    public void PauseHazard()
    {
        isActive = false;
    }

    public void ResumeHazard()
    {
        isActive = true;
    }

    public virtual void TriggerPhase()
    {
        switch (hazardPhase)
        {
            case 1:
                TriggerFirstPhase();
                break;
            case 2:
                TriggerSecondPhase();
                break;
            case 3:
                TriggerThirdPhase();
                break;
        }
    }

    private void SetVFX(bool objectStatus)
    {
        if (smokeObject != null)
            smokeObject.SetActive(objectStatus);
    }

    public virtual void TriggerFirstPhase()
    {
        Debug.Log("First phase triggered on " + gameObject);
        SetVFX(false);
    }

    public virtual void TriggerSecondPhase()
    {
        Debug.Log("Second phase triggered on " + gameObject);
        SetVFX(true);
    }

    public virtual void TriggerThirdPhase()
    {
       
    }

    private void OnDestroy()
    {
        hazardTimer?.Cancel();
        if (HazardManager.Instance != null)
        {
            HazardManager.Instance.UnregisterHazard(this);
        }
    }
}
