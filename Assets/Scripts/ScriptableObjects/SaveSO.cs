using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using JetBrains.Annotations;

public static class SaveSO
{
    [System.Serializable]
    public class LevelData
    {
        public string levelName;
        public float bestTime;
        public int levelIndex;
        public string coursePrefabName;
        public bool isSwinging;
        public List<float> allTimes;
        public List<float> campaignTimes;
    }

    public static void SaveLevel(LevelSO level)
    {
        string path = Path.Combine(Application.persistentDataPath, $"{level.levelName}_LevelData.PKSRson");
        string json = JsonUtility.ToJson(level);

        File.WriteAllText(path, json);
    }

    public static LevelSO LoadLevelData(LevelSO level)
    {
        string path = Path.Combine(Application.persistentDataPath, $"{level.levelName}_LevelData.PKSRson");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);

            // Populate the LevelSO instance with the data
            level.levelName = levelData.levelName;
            level.bestTime = levelData.bestTime;
            level.levelIndex = levelData.levelIndex;
            level.coursePrefabName = levelData.coursePrefabName;
            level.isSwinging = levelData.isSwinging;

            level.allTimes.Clear();
            if (levelData.allTimes != null)
            {
                level.allTimes = new List<float>(levelData.allTimes);
            }

            return level;
        }
        else
        {
            Debug.LogWarning($"No saved data found for {level.levelName}");
            return null;
        }
    }
}
