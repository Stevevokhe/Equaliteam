using UnityEngine;

public class FreeTheRadiatorCloth : MonoBehaviour
{
    [SerializeField] private Transform flyTarget;
    [SerializeField] private float flySpeed = 500f;
    [SerializeField] private float spinSpeed = 360f;

    private bool flying = false;
    private RectTransform rect;
    private FreeTheRadiatorManager manager;
    private Vector3 originalPosition;
    void Start()
    {
        manager = GameObject.FindAnyObjectByType<FreeTheRadiatorManager>();
        rect = GetComponent<RectTransform>();
        originalPosition = transform.position;
    }

    void Update()
    {
        if (flying)
        {
            // Spin
            rect.Rotate(0f, 0f, spinSpeed * Time.deltaTime);

            // Move toward target
            rect.anchoredPosition = Vector2.MoveTowards(
                rect.anchoredPosition,
                flyTarget.GetComponent<RectTransform>().anchoredPosition,
                flySpeed * Time.deltaTime
            );

            // Stop when arrived
            if (Vector2.Distance(rect.anchoredPosition,
                flyTarget.GetComponent<RectTransform>().anchoredPosition) < 1f)
            {
                flying = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Hand") && !flying)
        {
            //TODO play SFX here
            manager.ClothCleared();
            flying = true;
        }
    }
}
