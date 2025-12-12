using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    [Header("Game Ending Conditions")]
    [SerializeField] float gameEndTime;
    [SerializeField] float houseHealth;
    [SerializeField] float burnRate;
    private float currentTime,nrOfBurningHazards;
    private TextMeshProUGUI houseHealthValueText, timerValueText;
    private bool isBurning,isPaused,gameEnded = false;
    private HazardManager hazardManager;
    [SerializeField] private GameObject endGamePanel, houseSprite,pausePanel,optionsPanel;

    [Header("House Health Visuals")]
    [SerializeField] private Sprite houseIconLow;
    [SerializeField] private Sprite houseIconMid, houseIconHigh;
    [SerializeField] private Color32 healthHigh, healthMid, healthLow;

    [SerializeField] private Animator healthAnimator;

    private PlayerInput playerInput;
    private InputAction pauseAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        playerInput = GameObject.FindAnyObjectByType<PlayerController>().GetComponent<PlayerInput>();
        pauseAction = playerInput.actions["Pause"];
        pauseAction.performed += PauseByInput;
        playerInput.enabled =true;
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
        endGamePanel.SetActive(false);
        endGamePanel.SetActive(false);  
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
            endGamePanel.SetActive(true);
            gameEnded = true;
            endGamePanel.GetComponent<EndgameStatsManager>().GetGameoverStats(((int)houseHealth));
        }
        //House health decreasing calculations
        if (houseHealth<=0)
        {
            //Game Over
            Time.timeScale = 0f;
            endGamePanel.SetActive(true);
            gameEnded = true;
            endGamePanel.GetComponent<EndgameStatsManager>().GetGameoverStats(((int)houseHealth));
        } else if (isBurning)
        {
            houseHealth -= burnRate* nrOfBurningHazards * Time.deltaTime;
            houseHealth = Mathf.Clamp(houseHealth, 0f, 100f);
        }



        houseHealthValueText.SetText(((int)houseHealth) + "%");
        timerValueText.SetText(FormatTime(currentTime));

        ManageHouseVisuals();

    }

    public void StartBurning()
    {
        nrOfBurningHazards++;
        isBurning = true;

        // Animation handling
        healthAnimator.SetBool("HealthDropping", true);
    }

    public void StopBurning()
    {
        nrOfBurningHazards--;
        if(nrOfBurningHazards <= 0)
        {
            nrOfBurningHazards = 0;
            isBurning = false;

            // Animation handling
            healthAnimator.SetBool("HealthDropping", false);
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

    public void Pause()
    {
        if (!gameEnded)
        {
            isPaused = !isPaused;

            pausePanel.SetActive(isPaused);
            optionsPanel.SetActive(false);

            Time.timeScale = isPaused ? 0f : 1f;
        }
        
    }

    public void PauseByInput(InputAction.CallbackContext ctx)
    {
        Pause();
    }

    private void ManageHouseVisuals()
    {
        var houseSpriteComponent = houseSprite.GetComponent<Image>();
        if (houseHealth >= 50f)
        {
            houseSpriteComponent.sprite = houseIconHigh;
            houseHealthValueText.color = healthHigh;
        }
        else if (houseHealth >= 20 && houseHealth < 50)
        {
            houseSpriteComponent.sprite = houseIconMid;
            houseHealthValueText.color = healthMid;
        }
        else
        {
            houseSpriteComponent.sprite = houseIconLow;
            houseHealthValueText.color = healthLow;
        }
    }
}
