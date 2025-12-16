using UnityEngine;
using System.Collections.Generic;

public enum SFXType
{
    CatCrowl,
    CatMeow1,
    CatMeow2,
    CatPurr,
    RatGotSmacked,
    RatSqueek1,
    RatSqueek2,
    ElectricHum,
    ElectricSparkShot1,
    ElectricSparkShot2,
    ElectricSparks,
    FireBig,
    FireStartQuiet,
    FrameStartBurn,
    AnotherClick,
    BroomSwoosh,
    ChangeLightBulb,
    Click,
    DraggingSomething,
    FastBroomShoosh,
    FuseClick,
    LeakingWaterStart,
    Step1,
    Step2,
    Step3,
    FailGame,
    SuccesfullPuzzleCompleted,
    UIBackButton,
    UIHover1,
    UIHover2,
    UISelect,
    GrillDropped
}

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    [System.Serializable]
    public class SFXClip
    {
        public SFXType type;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
    }

    [SerializeField] private List<SFXClip> sfxClips = new List<SFXClip>();
    [SerializeField] private AudioSource audioSource;

    private Dictionary<SFXType, SFXClip> sfxDictionary = new Dictionary<SFXType, SFXClip>();

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDictionary()
    {
        // Create audio source if not assigned
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Populate dictionary
        foreach (var sfx in sfxClips)
        {
            if (!sfxDictionary.ContainsKey(sfx.type))
            {
                sfxDictionary.Add(sfx.type, sfx);
            }
            else
            {
                Debug.LogWarning($"Duplicate SFX type found: {sfx.type}");
            }
        }
    }

    public void PlaySFX(SFXType type)
    {
        if (sfxDictionary.TryGetValue(type, out SFXClip sfx))
        {
            if (sfx.clip != null)
            {
                audioSource.PlayOneShot(sfx.clip, sfx.volume);
            }
            else
            {
                Debug.LogWarning($"Audio clip for {type} is not assigned!");
            }
        }
        else
        {
            Debug.LogWarning($"SFX type {type} not found in dictionary!");
        }
    }

    public void PlaySFX(SFXType type, float volumeMultiplier)
    {
        if (sfxDictionary.TryGetValue(type, out SFXClip sfx))
        {
            if (sfx.clip != null)
            {
                audioSource.PlayOneShot(sfx.clip, sfx.volume * volumeMultiplier);
            }
            else
            {
                Debug.LogWarning($"Audio clip for {type} is not assigned!");
            }
        }
        else
        {
            Debug.LogWarning($"SFX type {type} not found in dictionary!");
        }
    }

    public void SetMasterVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    private void OnEnable()
    {
        EventBus.OnSFXCalled += PlaySFX;
    }

    private void OnDisable()
    {
        EventBus.OnSFXCalled -= PlaySFX;
    }
}