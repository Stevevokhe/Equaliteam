using UnityEngine;

public class LeakController : MonoBehaviour
{
    [Header("Scene References")]
    public Transform waterTube;      // cylinder
    public Transform puddleQuad;     // puddle quad on the floor

    [Header("Materials")]
    public Renderer waterTubeRenderer;
    public Renderer puddleRenderer;

    [Header("Timing")]
    public float tubeGrowTime = 0.6f;  // how long until water reaches the floor
    public float puddleDelay = 0.3f;  // wait after tube is full before puddle starts
    public float puddleGrowTime = 1.0f;  // how long puddle grows in

    [Header("Opacity Targets")]
    public float tubeOpacity = 1.0f;
    public float puddleOpacity = 0.95f;

    Vector3 tubeBaseScale;
    Vector3 puddleBaseScale;

    Material tubeMat;
    Material puddleMat;

    bool playing;

    private void Start()
    {
        PlayLeak();
    }
    void Awake()
    {
        // Cache base scales
        tubeBaseScale = waterTube.localScale;
        puddleBaseScale = puddleQuad.localScale;

        // Get unique material instances so we don't affect other objects
        if (waterTubeRenderer != null)
            tubeMat = waterTubeRenderer.material;
        if (puddleRenderer != null)
            puddleMat = puddleRenderer.material;

        // Start hidden
        ResetState();
    }

    public void ResetState()
    {
        // Tube starts collapsed & invisible
        waterTube.localScale = new Vector3(0f, 0f, 0f);
        if (tubeMat != null)
            tubeMat.SetFloat("_Opacity", 0f);

        // Puddle starts tiny & invisible
        puddleQuad.localScale = Vector3.zero;
        if (puddleMat != null)
            puddleMat.SetFloat("_Alpha", 0f);

        playing = false;
    }

    // Call this from another script, a trigger, or the Inspector (via UnityEvent)
    public void PlayLeak()
    {
        if (playing) return;
        StartCoroutine(PlayLeakRoutine());
    }

    System.Collections.IEnumerator PlayLeakRoutine()
    {
        playing = true;

        // 1) Grow the water tube from top to bottom
        float t = 0f;
        while (t < tubeGrowTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / tubeGrowTime);

            waterTube.localScale = new Vector3(
                Mathf.Lerp(0f, tubeBaseScale.x, k),
                Mathf.Lerp(0f, tubeBaseScale.y, k),
                Mathf.Lerp(0f, tubeBaseScale.z, k)
            );

            if (tubeMat != null)
                tubeMat.SetFloat("_Opacity", Mathf.Lerp(0f, tubeOpacity, k));

            yield return null;
        }

        // 2) Wait a bit with full stream before puddle starts
        if (puddleDelay > 0f)
            yield return new WaitForSeconds(puddleDelay);

        // 3) Grow puddle size & alpha
        t = 0f;
        while (t < puddleGrowTime)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / puddleGrowTime);

            puddleQuad.localScale = Vector3.Lerp(Vector3.zero, puddleBaseScale, k);

            if (puddleMat != null)
                puddleMat.SetFloat("_Alpha", Mathf.Lerp(0f, puddleOpacity, k));

            yield return null;
        }

        playing = false;
    }
}
