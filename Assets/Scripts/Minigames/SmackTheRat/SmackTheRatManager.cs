using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmackTheRatManager : Minigame
{
    [SerializeField] private GameObject ratImage;
    [SerializeField] private List<RectTransform> ratPositions;
    [SerializeField] private float moveSpeed = 200f; // Speed of rat movement
    [SerializeField] private float escapeMoveSpeed = 500f; // Speed when escaping after being hit
    [SerializeField] private int clicksNeeded = 3;

    private Button ratButton;
    private RectTransform ratRectTransform;
    private int currentClicks = 0;
    private Vector2 targetPosition;
    private bool isMoving = false;
    private float currentMoveSpeed;
    void Awake()
    {
        ratButton = ratImage.GetComponent<Button>();
        if (ratButton == null)
        {
            ratButton = ratImage.AddComponent<Button>();
        }

        ratRectTransform = ratImage.GetComponent<RectTransform>();
        currentMoveSpeed = moveSpeed;
    }

    void Start()
    {
        ratButton.onClick.AddListener(OnRatClicked);
    }

    public override void StartMinigame()
    {
        currentClicks = 0;
        ratImage.SetActive(true);
        isMoving = true;

        int startIndex = Random.Range(0, ratPositions.Count);
        ratRectTransform.anchoredPosition = ratPositions[startIndex].anchoredPosition;

        PickNewTarget(startIndex);

        StartCoroutine(ContinuousMovement());
    }

    IEnumerator ContinuousMovement()
    {
        while (isMoving && currentClicks < clicksNeeded)
        {
            ratRectTransform.anchoredPosition = Vector2.MoveTowards(
                ratRectTransform.anchoredPosition,
                targetPosition,
                currentMoveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(ratRectTransform.anchoredPosition, targetPosition) < 1f)
            {
                PickNewTarget();
                currentMoveSpeed = moveSpeed;
            }

            yield return null;
        }
    }

    void PickNewTarget(int excludeIndex = -1)
    {
        if (ratPositions.Count == 0) return;

        int randomIndex;
        int attempts = 0;

        do
        {
            randomIndex = Random.Range(0, ratPositions.Count);
            attempts++;
        }
        while (randomIndex == excludeIndex && ratPositions.Count > 1 && attempts < 10);

        targetPosition = ratPositions[randomIndex].anchoredPosition;
    }

    void OnRatClicked()
    {
        currentClicks++;
        Debug.Log($"Rat clicked! {currentClicks}/{clicksNeeded}");

        if (currentClicks >= clicksNeeded)
        {
            CompleteMinigame();
        }
        else
        {
            PickNewTarget();
            currentMoveSpeed = escapeMoveSpeed;

            StartCoroutine(ClickAnimation());
        }
    }

    IEnumerator ClickAnimation()
    {
        Vector3 originalScale = ratRectTransform.localScale;
        ratRectTransform.localScale = originalScale * 0.8f;
        yield return new WaitForSeconds(0.1f);
        ratRectTransform.localScale = originalScale;
    }

    void CompleteMinigame()
    {
        isMoving = false;
        EventBus.InvokeOnMinigameCompleted();

        ratImage.SetActive(false);
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (ratButton != null)
        {
            ratButton.onClick.RemoveListener(OnRatClicked);
        }
    }
}