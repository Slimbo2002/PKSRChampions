using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUI : MonoBehaviour
{
    public LoadingScreen loadingScreen;

    public List<LevelSO>levels= new List<LevelSO>();
    int nextLevelIndex;

    [Header("FinishScreen")]
    public TextMeshProUGUI[] finishScreenText;

    [Header("Pause")]
    public TextMeshProUGUI[] pauseScreenText;

    public GameObject pauseMenu;

    [Header("EntryScreen")]
    public Camera sceneryCam, playerCam;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] GameObject entryScreen;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        sceneryCam.enabled = true;
        playerCam.enabled = false;

        titleText.text = GameManager.Instance.level.levelName;

        loadingScreen = GameManager.Instance.loadScreen;

        for(int i = 0; i < 3; i++)
        {
            SetCampaignTimes(finishScreenText[i], GameManager.Instance.level.campaignTimes[i].ToString("F3"));
            SetCampaignTimes(pauseScreenText[i], GameManager.Instance.level.campaignTimes[i].ToString("F3"));
        }
        
    }
    private void Update()
    {
        if (UserInputs.inputREF.pauseInput && !GameManager.Instance.waitingForCountdown)
        {
            Paused();
        }

        if (entryScreen.activeSelf)
        {
            if (UserInputs.inputREF.anyInput)
            {
                Play();
            }
        }
    }
    public void ToMainMenu()
    {
        loadingScreen.LoadScene(0);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        GameManager.Instance.isPaused= false;
        GameManager.Instance.endScreen.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        GameManager.Instance.playerRef.GetComponent<ResetPosition>().ResetPos();
    }

    public void NextLevel()
    {
        nextLevelIndex = GameManager.Instance.level.levelIndex + 1;

        if(nextLevelIndex <= levels.Count)
        {
            GameManager.Instance.level = levels[nextLevelIndex]; 
            loadingScreen.LoadScene(1);
        }
        else
        {
            loadingScreen.LoadScene(0);
        }
        
    }
    public void Paused()
    {
        GameManager.Instance.isPaused = !GameManager.Instance.isPaused;
        pauseMenu.SetActive(GameManager.Instance.isPaused);
        GameManager.Instance.gameUI.SetActive(!GameManager.Instance.isPaused);

        Time.timeScale = GameManager.Instance.isPaused ? 0f : 1.0f;

        Cursor.lockState = GameManager.Instance.isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = GameManager.Instance.isPaused;
    }
    public void Play()
    {
        sceneryCam.enabled= false;
        playerCam.enabled = true;

        entryScreen.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StartCoroutine(GameManager.Instance.timing.StartCountdown());
    }

    void SetCampaignTimes(TextMeshProUGUI text, string textToBe)
    {
        text.text = textToBe;
    }
}
