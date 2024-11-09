using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelSO", order = 1)]
public class LevelSO : ScriptableObject
{
    public string levelName;
    public float bestTime;
    public int levelIndex;
    public string leaderboardKey;
    public bool isSwinging;

    public string coursePrefabName;

    public List<float> allTimes = new List<float>();
    public List<float> campaignTimes = new List<float>();

    // Load the level data explicitly
    public void LoadLevelData()
    {
        LevelSO levelData = SaveSO.LoadLevelData(this);

        if (levelData != null)
        {
            levelName = levelData.levelName;
            bestTime = levelData.bestTime;
            levelIndex = levelData.levelIndex;
            coursePrefabName = levelData.coursePrefabName;
            allTimes = new List<float>(levelData.allTimes);
        }
        else
        {
            Debug.LogWarning("No save data found for level: " + levelName);
        }
    }




}
