using System.Collections;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public LoadingScreen loadScreen;
    public GameObject timer;
    public Timing timing;

    Transform courseSpawnPoint;
    public GameObject playerRef;

    public bool isPaused;
    public float savedTime;
    public LevelSO level;

    public GameObject endScreen;
    public GameObject gameUI;
    public GameObject countdownPage;
    public TextMeshProUGUI endScreenTime;

    public bool waitingForCountdown;

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

        level = LevelSOTransfer.Instance.currentLevelSO;
        GameObject coursePrefab = Resources.Load<GameObject>(level.coursePrefabName);


        Instantiate(coursePrefab, courseSpawnPoint);
        

        playerRef.GetComponent<ResetPosition>().spawnPos = coursePrefab.transform.Find("SpawnPoint");

        InitPlayerAbilities();
    }
    private void Start()
    {
        playerRef.GetComponent<ResetPosition>().SetPos();
        Time.timeScale= 1.0f;
        timing = timer.GetComponent<Timing>();
        waitingForCountdown = true;
    }
    void Update()
    {

    }

    void InitPlayerAbilities()
    {
        //Swinging
        playerRef.GetComponent<Swinging>().enabled = level.isSwinging;
        playerRef.GetComponent<Swinging>().predictionPoint.gameObject.SetActive(level.isSwinging);
        playerRef.GetComponent<LineRenderer>().enabled= level.isSwinging;

        playerRef.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void StartTimer()
    {
        timing.timerActive = true;
        
    }
    public void StopTimer()
    {
        timing.timerActive = false;
        timing.SaveTime();
        StartCoroutine(FinishCourse());

    }
    public void RestartTimer()
    {
        timing.timerActive = false;
        timing.timing = 0f;
        countdownPage.SetActive(true);
        gameUI.SetActive(false);
    }

    public float GetTiming()
    {
        return timing.timing;
    }
    IEnumerator FinishCourse()
    {
        waitingForCountdown = true;
        Time.timeScale = 0.3f;

        yield return new WaitForSecondsRealtime(1.3f);

        Time.timeScale = 0f;
        isPaused = true;
        LevelComplete();
    }

    void LevelComplete()
    {
        endScreenTime.text = timing.savedTime.ToString("F3");
        gameUI.SetActive(false);
        endScreen.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        string username = "";

        if(PlayerPrefs.GetString("Username") == "")
        {
            username = "user";
        }
        else
        {
            username = PlayerPrefs.GetString("Username");
        }

        //Leaderboard.Instance.SetLeaderboardEntry(level.leaderboardKey, username, timing.savedTime);
        SaveSO.SaveLevel(level);
    }

    string GetUsername()
    {
        return string.IsNullOrEmpty(PlayerPrefs.GetString("Username")) ? "user" : PlayerPrefs.GetString("Username");
    }




}
