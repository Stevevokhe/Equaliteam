using UnityEngine;

public class OutletSparks : MonoBehaviour
{
    [Header("Particles")]
    public ParticleSystem sparks;

    [Header("Light Flicker")]
    public Light pointLight;
    public float flashIntensity = 2f;   // how bright during a spark
    public float fadeSpeed = 5f;        // how fast it fades back down

    [Header("Random Burst Timing")]
    public float minDelay = 0.2f;
    public float maxDelay = 1.5f;

    private float timer;

    void Start()
    {
        if (sparks == null)
            sparks = GetComponent<ParticleSystem>();

        ResetTimer();
    }

    void Update()
    {
        // Fade light back to 0 over time
        if (pointLight != null)
        {
            pointLight.intensity = Mathf.Lerp(
                pointLight.intensity,
                0f,
                Time.deltaTime * fadeSpeed
            );
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // Play spark burst
            if (sparks != null)
                sparks.Play();

            // Flash the light
            if (pointLight != null)
                pointLight.intensity = flashIntensity;

            ResetTimer();
        }
    }

    void ResetTimer()
    {
        timer = Random.Range(minDelay, maxDelay);
    }
}

