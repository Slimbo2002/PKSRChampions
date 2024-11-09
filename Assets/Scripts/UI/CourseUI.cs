using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class CourseUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public LevelSO level;
    public Leaderboard leaderboard;

    public TextMeshProUGUI text;
    public Color[] material;


    LevelSO levelData;
    void Update()
    { 
        
    }
    void Start()
    {
        // Load level data directly into `level`
        if (!SaveSO.LoadLevelData(level))
        {
            Debug.LogWarning($"No saved data found for {level.levelName}. Initializing with default values.");
            level.bestTime = 0f; // Default best time
        }

        // Update color based on loaded data
        UpdateColor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var mainMenu = MainMenu.Instance;

        if (mainMenu == null)
        {
            Debug.LogError("MainMenu.Instance is null. Make sure the MainMenu has been initialized before accessing it.");
            return;
        }

        // Set the levelHover to the loaded or initialized data
        mainMenu.levelHover = level;
        MainMenu.Instance.leaderboard.GetLeaderboard(level.leaderboardKey);
        MainMenu.Instance.leaderboard.GetPersonalEntry(level.leaderboardKey);
        mainMenu.UpdateCampaignTimes(level.campaignTimes[0], level.campaignTimes[1], level.campaignTimes[2], level.bestTime);
    }


    public void UpdateColor()
    {
        if(level.bestTime != 0)
        {
            // Assuming materials array follows the order: Gold, Silver, Bronze
            for (int i = 0; i < level.campaignTimes.Count; i++)
            {
                // Check if the saved time is less than the current campaign time
                if (level.bestTime < level.campaignTimes[i])
                {
                    Debug.Log("Time Achieved: " + (i == 0 ? "Gold" : i == 1 ? "Silver" : "Bronze"));

                    // Assign the corresponding material to the text based on the time achieved
                    text.color = material[i];
                    break; // Exit the loop once the correct material is assigned
                }
            }
        }
        

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MainMenu.Instance.ResetBestTimes();
        MainMenu.Instance.levelHover = null;
        MainMenu.Instance.leaderboard.ResetLeaderboard();
    }
}
