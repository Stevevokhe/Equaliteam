using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Ending Conditions")]
    [SerializeField] float gameEndTime;
    [SerializeField] float houseHealth;
    [SerializeField] float burnRate;
    private float currentTime,nrOfBurningHazards;
    private TextMeshProUGUI houseHealthValueText, timerValueText;
    private bool isBurning = false;
    private HazardManager hazardManager;
    [SerializeField] private GameObject gameOverPanel,victoryPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        hazardManager = FindAnyObjectByType<HazardManager>();
        hazardManager.OnHouseStartedBurning += StartBurning;
        hazardManager.OnHouseStoppedBurning += StopBurning;
    }

    private void OnDisable()
    {
        hazardManager.OnHouseStartedBurning -= StartBurning;
        hazardManager.OnHouseStoppedBurning -= StopBurning;
    }

    void Start()
    {
        houseHealthValueText = GameObject.Find("HouseHealthValue").GetComponent<TextMeshProUGUI>();
        timerValueText = GameObject.Find("TimerValue").GetComponent<TextMeshProUGUI>();
        currentTime = gameEndTime;
        Time.timeScale = 1f;
        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);  
    }

    // Update is called once per frame
    void Update()
    {
        //Timer decreasing calculations
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Max(currentTime, 0f); 
        }
        //Victory
        if(currentTime <= 0)
        {
            Time.timeScale = 0f;
            victoryPanel.SetActive(true);
        }
        //House health decreasing calculations
        if (houseHealth<=0)
        {
            //Game Over
            Time.timeScale = 0f;
            gameOverPanel.SetActive(true);
        } else if (isBurning)
        {
            houseHealth -= burnRate* nrOfBurningHazards * Time.deltaTime;
            houseHealth = Mathf.Clamp(houseHealth, 0f, 100f);
        }



        houseHealthValueText.SetText($"{houseHealth:0.0}%");
        timerValueText.SetText(FormatTime(currentTime));


    }

    public void StartBurning()
    {
        nrOfBurningHazards++;
        isBurning = true;
    }

    public void StopBurning()
    {
        nrOfBurningHazards--;
        if(nrOfBurningHazards <= 0)
        {
            nrOfBurningHazards = 0;
            isBurning = false;
        }
        
    }

    public static string FormatTime(float totalSeconds)
    {
        int minutes = (int)(totalSeconds / 60);
        int seconds = (int)(totalSeconds % 60);
        return $"{minutes}:{seconds:00}";
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

}
