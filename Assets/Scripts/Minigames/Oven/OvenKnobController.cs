using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OvenKnobController : MonoBehaviour, IDragHandler
{
    [SerializeField] private Transform handle;
    [SerializeField] private Image fill;
    [SerializeField] private float minAngle,maxAngle;
    private Vector3 mousePos;
    private bool isDone;
    private OvenManager manager;
    private float angle;
    private void Awake()
    {
        manager = GameObject.FindAnyObjectByType<OvenManager>();
        Quaternion r = Quaternion.AngleAxis(0 + 135f, Vector3.forward);
        handle.rotation = r;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDone)
        {
            mousePos = Input.mousePosition;
            Vector2 dir = mousePos - handle.position;
            angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            angle = (angle <= 0) ? (360 + angle) : angle;

            if (angle >= minAngle || angle >= maxAngle)
            {
                Quaternion r = Quaternion.AngleAxis(angle, Vector3.forward);
                handle.rotation = r;
                angle = ((angle >= maxAngle) ? (angle - 360) : angle) + 45;
                fill.fillAmount = 0.75f - (angle / 360f);
            }

            if (angle >= maxAngle)
            {
                isDone = true;
                manager.KnobCompleted();
            }
        }

        
    }
}
