using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    public Leaderboard leaderboard;
    public Button courses;


    public TextMeshProUGUI goldText;
    public TextMeshProUGUI silverText;
    public TextMeshProUGUI bronzeText;
    public TextMeshProUGUI pbTime;

    public Material colours;

    public LevelSO levelHover;

    public GameObject title;
    public GameObject courseSelectActive;
    public GameObject titleScreen;
    public GameObject singlePlayerModesScreen;
    public GameObject optionsScreen;
    public GameObject TrainingGroundsScreen;

    public GameObject nextPage;
    

    public LoadingScreen loadingScreen;
    public LevelSO[] levels; // Array of ScriptableObjects to initialize

    public menuPage page;
    public enum menuPage
    {
        Title,
        SingleplayerModes,
        CourseSelect,
        Options,
        TrainingGrounds
    }
    public enum buttonType
    {
        SinglePlayer,
        Options,
        Quit,
        Multiplayer,
        Courses
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeScriptableObjects();
    }
    private void Start()
    {
        UserInputs.inputREF.enabled = true;
        page = menuPage.Title;
        Time.timeScale = 1;
    }

    void InitializeScriptableObjects()
    {
        foreach (var level in levels)
        {
            if (level != null)
            {
                level.LoadLevelData(); // or whatever initialization logic you have
            }
        }
        // You can also add any other initialization logic here, if needed.
    }

    public void TrainingGroundsButton()
    {
        singlePlayerModesScreen.SetActive(false);
        TrainingGroundsScreen.SetActive(true);
        title.SetActive(false);

        page = menuPage.TrainingGrounds;
    }
    public void SinglePlayerButton()
    {
        titleScreen.SetActive(false);
        singlePlayerModesScreen.SetActive(true);
        page = menuPage.SingleplayerModes;
    }
    public void CourseSelectScreen()
    {
        title.SetActive(false);
        singlePlayerModesScreen.SetActive(false);
        courseSelectActive.SetActive(true);
        page = menuPage.CourseSelect;
    }
    public void QuitButton()
    {
        Application.Quit();
    }
    public void LoadLevel1()
    {
        TrainingGroundsScreen.SetActive(false);

        LevelSOTransfer.Instance.currentLevelSO = levelHover;

        loadingScreen.LoadScene(1);
    }

    public void UpdateCampaignTimes(float goldTime, float silverTime, float bronzeTime, float bestTime)
    {
        // Update the UI text fields with the level times
        goldText.text = goldTime.ToString("F2");
        silverText.text = silverTime.ToString("F2");
        bronzeText.text = bronzeTime.ToString("F2");

        pbTime.text = bestTime.ToString("F3");

    }

    public void OptionsButton()
    {
        page = menuPage.Options;
        optionsScreen.SetActive(true);
        titleScreen.SetActive(false);
    }

    public void BackButton()
    {
        switch (page) 
        {
            case menuPage.Options:
                optionsScreen.SetActive(false);
                titleScreen.SetActive(true);
                page = menuPage.Title;
                break;
            case menuPage.SingleplayerModes:
                page = menuPage.Title;
                singlePlayerModesScreen.SetActive(false);
                titleScreen.SetActive(true);
                break;
            case menuPage.CourseSelect:
                page = menuPage.SingleplayerModes;
                courseSelectActive.SetActive(false);
                singlePlayerModesScreen.SetActive(true);
                title.SetActive(true);
                break;
            case menuPage.TrainingGrounds:
                page = menuPage.SingleplayerModes;
                TrainingGroundsScreen.SetActive(false);
                singlePlayerModesScreen.SetActive(true);
                title.SetActive(true);
                break;



        }

            
                
    }

    public void ResetBestTimes()
    {
        goldText.text = "";
        silverText.text = "";
        bronzeText.text = "";
        pbTime.text = "";
    }



}
