using UnityEngine;

public class FreeTheRadiatorHand : MonoBehaviour
{
    [SerializeField] private RectTransform leftPoint;
    [SerializeField] private RectTransform rightPoint;
    [SerializeField] private float horizontalSpeed = 200f;
    [SerializeField] private float verticalSpeed = 400f;
    [SerializeField] private Transform startingPosition;
    private RectTransform rect;
    private bool movingRight = true;
    private bool movingHorizontally = true;
    private float initialY;

    private bool movingDown = false;
    private bool movingUp = false;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        initialY = rect.anchoredPosition.y;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Mouse0) && movingHorizontally)
        {
            movingHorizontally = false;
            movingDown = true;

        }

        if (movingHorizontally)
            MoveHorizontal();
        else
            MoveVertical();
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

    void MoveVertical()
    {
        Vector2 pos = rect.anchoredPosition;

        if (movingDown)
        {
            pos.y -= verticalSpeed * Time.deltaTime;
        }
        else if (movingUp)
        {
            pos.y += verticalSpeed * Time.deltaTime;
            if (pos.y >= initialY)
            {
                pos.y = initialY;
                movingUp = false;
                movingHorizontally = true;
            }
        }

        rect.anchoredPosition = pos;
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (!movingDown) return;

        if (col.CompareTag("Cloth"))
        {
            // Cloth handles itself.
            ReturnUp();
        }
        else
        {
            // Hit something else, go back up.
            ReturnUp();
        }
    }

    public void ResetHand()
    {
        transform.position = startingPosition.position;
        movingHorizontally = true;
    }

    void ReturnUp()
    {
        movingDown = false;
        movingUp = true;
    }
}
