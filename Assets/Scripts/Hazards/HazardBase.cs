using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HazardBase : MonoBehaviour
{
    public string hazardName;
    public int hazardPhase = 1;
    public PlayerTool requiredTool;
    [SerializeField] private VFXManager vFXManager;
    public float internalTimer;
    public float secondPhaseThreshold = 10f;
    public float thirdPhaseThreshold = 30f;

    [Header("Timer UI")]
    [SerializeField] private GameObject SecondPhaseTimerObject;
    [SerializeField] private Image TimerImage;
    [SerializeField] private GameObject ThirdPhaseTimerObject;

    [Header("Pulse Settings")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseMinScale = 0.8f;
    [SerializeField] private float pulseMaxScale = 1.2f;

    private bool isActive = false;
    private bool isPaused = false;
    private Coroutine pulseCoroutine;
    private Vector3 originalScale;

    public bool IsActive => isActive;

    private void Start()
    {
        SetTimerUI(false);
        InitializeHazard();

        if (ThirdPhaseTimerObject != null)
        {
            originalScale = ThirdPhaseTimerObject.transform.localScale;
        }
    }

    protected virtual void InitializeHazard()
    {
        if (HazardManager.Instance != null)
        {
            HazardManager.Instance.RegisterHazard(this);
        }
    }

    public void ActivateHazard()
    {
        if (!isActive)
        {
            isActive = true;
            internalTimer = 0f;
            hazardPhase = 1;
            TriggerPhase();

            OnHazardActivated();
        }
    }

    public void UpdateHazard(float deltaTime)
    {
        if (!isActive || isPaused) return;

        internalTimer += deltaTime;

        if (hazardPhase == 2)
        {
            UpdateTimerFill();
        }

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

    private void UpdateTimerFill()
    {
        if (TimerImage != null)
        {
            float phase2Duration = thirdPhaseThreshold - secondPhaseThreshold;
            float phase2Progress = (internalTimer - secondPhaseThreshold) / phase2Duration;

            phase2Progress = Mathf.Clamp01(phase2Progress);

            TimerImage.fillAmount = phase2Progress;
        }
    }

    private void SetTimerUI(bool active)
    {
        if (SecondPhaseTimerObject != null)
        {
            SecondPhaseTimerObject.SetActive(active);
        }
    }

    private void SetThirdPhaseUI(bool active)
    {
        if (ThirdPhaseTimerObject != null)
        {
            ThirdPhaseTimerObject.SetActive(active);
        }
    }

    public void ResolveHazard()
    {
        if (!isActive) return;

        isActive = false;
        internalTimer = 0f;
        hazardPhase = 1;
        vFXManager.DeactivateAllVFX();
        SetTimerUI(false);
        SetThirdPhaseUI(false);
        StopPulseCoroutine();

        OnHazardResolved();
    }

    public void RemoveHazardCompletely()
    {
        isActive = false;
        StopPulseCoroutine();
        UnregisterFromManager();
        Destroy(gameObject);
    }

    private void UnregisterFromManager()
    {
        if (HazardManager.Instance != null)
        {
            HazardManager.Instance.UnregisterHazard(this);
        }
    }

    protected virtual void OnHazardActivated()
    {
        Debug.Log($"Hazard activated: {hazardName}");
    }

    protected virtual void OnHazardResolved()
    {
        Debug.Log($"Hazard resolved: {hazardName}");

        // Notify the HazardManager that this hazard was resolved
        if (HazardManager.Instance != null)
        {
            HazardManager.Instance.StopHouseBurning();
            HazardManager.Instance.OnHazardResolved(this);
        }
    }

    public void DeactivateHazard()
    {
        if (isActive)
        {
            isActive = false;
            internalTimer = 0f;
            hazardPhase = 1;
            vFXManager.DeactivateAllVFX();
            SetTimerUI(false);
            SetThirdPhaseUI(false);
            StopPulseCoroutine();
        }
    }

    public int GetHazardPhase()
    {
        return hazardPhase;
    }

    public bool CheckToolRequirement(PlayerTool heldTool)
    {
        if (requiredTool == PlayerTool.None) return true;
        else return heldTool == requiredTool;
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
        isActive = false;
        SetTimerUI(false);
        StopPulseCoroutine();
        TriggerPhase();
    }

    public void PauseHazard()
    {
        isPaused = true;
    }

    public void ResumeHazard()
    {
        isPaused = false;
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

    //private void SetVFX(bool objectStatus)
    //{
        //if (smokeObject != null)
            //smokeObject.SetActive(objectStatus);
    //}

    public virtual void TriggerFirstPhase()
    {
        Debug.Log("First phase triggered on " + gameObject);
        SetTimerUI(false);
        SetThirdPhaseUI(false);
        StopPulseCoroutine();
    }

    public virtual void TriggerSecondPhase()
    {
        Debug.Log("Second phase triggered on " + gameObject);
        SetTimerUI(true);
        StopPulseCoroutine();

        vFXManager.ActivateStage1VFX();

        if (TimerImage != null)
        {
            TimerImage.fillAmount = 0f;
        }
    }

    public virtual void TriggerThirdPhase()
    {
        HazardManager.Instance.StartHouseBurning();
        SetThirdPhaseUI(true);
        SetTimerUI(false);
        StartPulseCoroutine();

        vFXManager.ActivateStage2VFX();
        vFXManager.DeactivateStage1VFX();
    }

    private void StartPulseCoroutine()
    {
        if (ThirdPhaseTimerObject != null)
        {
            StopPulseCoroutine();
            pulseCoroutine = StartCoroutine(PulseThirdPhaseObject());
        }
    }

    private void StopPulseCoroutine()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;

            if (ThirdPhaseTimerObject != null)
            {
                ThirdPhaseTimerObject.transform.localScale = originalScale;
            }
        }
    }

    private IEnumerator PulseThirdPhaseObject()
    {
        while (ThirdPhaseTimerObject != null && ThirdPhaseTimerObject.activeInHierarchy)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * pulseSpeed;
                float scale = Mathf.Lerp(pulseMinScale, pulseMaxScale, Mathf.PingPong(t, 1f));
                ThirdPhaseTimerObject.transform.localScale = originalScale * scale;
                yield return null;
            }
        }
    }

    private void OnDestroy()
    {
        StopPulseCoroutine();
        UnregisterFromManager();
    }
}