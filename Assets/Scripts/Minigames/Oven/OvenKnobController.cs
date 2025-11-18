using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OvenKnobController : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [Header("References")]
    [SerializeField] private Transform handle;
    [SerializeField] private Image fill;

    [Header("Limits (0..360). 0 = top")]
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;

    [Header("Done")]
    [SerializeField] private float doneThreshold = 0.5f;

    private Vector2 centerPoint;
    private bool isDone;
    private OvenManager manager;
    
    private void Awake()
    {
        manager = GameObject.FindAnyObjectByType<OvenManager>();
        
        float randomAngle = Random.Range(20f, 300f);
        handle.localEulerAngles = new Vector3(0f, 0f, -randomAngle);
        fill.fillAmount = randomAngle/360;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Calculate center of knob in screen coordinates
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)handle,
            handle.position,
            eventData.pressEventCamera,
            out centerPoint
        );
        centerPoint = handle.TransformPoint(centerPoint);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDone)
        {
            // convert pointer to world/screen position, then compute direction from knob centre
            Vector2 pointerScreenPos = eventData.position;

            // knob center in screen coordinates
            Vector2 knobScreenPos = handle.position;

            Vector2 dir = pointerScreenPos - knobScreenPos;

            if (dir.sqrMagnitude < 0.0001f) return; // avoid zero-length

            // Calculate angle so that 0 is UP and increases clockwise (0..360)
            // Using Atan2(x, y) gives angle from up, but result is -180..180
            float rawAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg; // 0 = up
            float angle360 = (rawAngle + 360f) % 360f; // convert to 0..360

            // Clamp into the allowed sector (supports sectors that wrap around)
            float clamped = ClampAngleInSector(angle360, minAngle, maxAngle);

            if (!IsAngleBetween(angle360, minAngle, maxAngle))
            {
                // Do NOT update knob rotation when outside range
                return;
            }

            // Apply rotation: we negate the angle because Unity's positive Z rotation is CCW.
            handle.localEulerAngles = new Vector3(0f, 0f, -clamped);
            fill.fillAmount = clamped/360;
            // Debug "done" when near zero (considering wrap)
            float diffToZero = Mathf.Min(Mathf.Abs(Mathf.DeltaAngle(clamped, 0f)), Mathf.Abs(Mathf.DeltaAngle(clamped, 360f)));
            if (diffToZero <= doneThreshold)
            {
                isDone = true;
                manager.KnobCompleted();
            }
            
        }

        
    }

    private float ClampAngleInSector(float angle, float min, float max)
    {
        angle = Normalize360(angle);
        min = Normalize360(min);
        max = Normalize360(max);

        if (IsAngleBetween(angle, min, max))
            return angle;

        // pick the closer boundary (on the circle)
        float distToMin = Mathf.Abs(Mathf.DeltaAngle(angle, min));
        float distToMax = Mathf.Abs(Mathf.DeltaAngle(angle, max));

        return (distToMin < distToMax) ? min : max;
    }

    private static float Normalize360(float a)
    {
        return (a % 360f + 360f) % 360f;
    }

    private static bool IsAngleBetween(float angle, float min, float max)
    {
        angle = (angle % 360f + 360f) % 360f;
        min = (min % 360f + 360f) % 360f;
        max = (max % 360f + 360f) % 360f;

        if (min <= max)
            return angle >= min && angle <= max;

        // wrap case, e.g. min=300, max=60 -> true if angle >=300 or angle <=60
        return angle >= min || angle <= max;
    }
}
