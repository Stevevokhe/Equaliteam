using UnityEngine;

public class GrilDropController : MonoBehaviour
{
    [Header("Object Setup")]
    public Transform attachPoint;          
    public GameObject objectToHold;
    public RectTransform leftPoint;
    public RectTransform rightPoint;
    public float horizontalSpeed = 400f;
    public float droppingForce = 20f;

    [Header("Release Settings")]
    public bool releaseOnTrigger = false;  // Set true externally to release

    private bool isHolding = true;
    private Rigidbody2D rb2D;
    private bool movingRight = true;
    private bool movingHorizontally = true;
    private GrilDropManager manager;
    private RectTransform rect;
    void Start()
    {
        manager = GameObject.FindAnyObjectByType<GrilDropManager>();
        rect = GetComponent<RectTransform>();

        // Prepare physics if present
        rb2D = objectToHold.GetComponent<Rigidbody2D>();

        // Freeze physics while holding
        SetPhysicsActive(false);

        // Snap to attach point immediately
        SnapObjectToAttachPoint();
    }

    void Update()
    {
        if (isHolding)
            SnapObjectToAttachPoint();

        if (Input.GetKeyUp(KeyCode.Mouse0) && movingHorizontally)
        {
            movingHorizontally = false;
            ReleaseObject();
        }

        if (movingHorizontally)
            MoveHorizontal();


        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            ResetGrillTop();
        }
    }

    void MoveHorizontal()
    {
        Vector2 pos = rect.anchoredPosition;

        if (movingRight)
        {
            pos.x += horizontalSpeed * Time.deltaTime;
            if (pos.x >= rightPoint.anchoredPosition.x)
            {
                pos.x = rightPoint.anchoredPosition.x;
                movingRight = false;
            }
        }
        else
        {
            pos.x -= horizontalSpeed * Time.deltaTime;
            if (pos.x <= leftPoint.anchoredPosition.x)
            {
                pos.x = leftPoint.anchoredPosition.x;
                movingRight = true;
            }
        }

        rect.anchoredPosition = pos;
    }

    public void ResetGrillTop()
    {
        movingHorizontally = true;
        isHolding = true;
        objectToHold.transform.position = attachPoint.position;
        objectToHold.transform.rotation = Quaternion.identity;
        SetPhysicsActive(false);
    } 

    public void SnapObjectToAttachPoint()
    {
        objectToHold.transform.position = attachPoint.position;
        objectToHold.transform.rotation = attachPoint.rotation;
    }

    public void ReleaseObject()
    {
        if (!isHolding) return;

        isHolding = false;
        releaseOnTrigger = false;
        rb2D.AddForce(Vector2.down * droppingForce, ForceMode2D.Force);
        SetPhysicsActive(true);
    }

    public void SetPhysicsActive(bool active)
    {
        if (rb2D != null)
        {
            if(active)
            rb2D.bodyType = RigidbodyType2D.Dynamic;
            else
            rb2D.bodyType = RigidbodyType2D.Kinematic;

            rb2D.simulated = active;
        }
        
        
    }    
}
