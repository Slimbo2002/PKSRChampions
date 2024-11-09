using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;
using System;

public class Leaderboard : MonoBehaviour
{
    public static Leaderboard Instance;

    public List<TextMeshProUGUI> names = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> scores = new List<TextMeshProUGUI>();
    public List<TextMeshProUGUI> rank = new List<TextMeshProUGUI>();

    public TextMeshProUGUI mineRank;
    public TextMeshProUGUI mineName;
    public TextMeshProUGUI mineScore;

    public string publicLeaderboardKey;

    public Color mineColor = new Color();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetLeaderboard(string publicStringKey)
    {
        LeaderboardCreator.GetLeaderboard(publicStringKey, (msg) =>
        {
            if (msg == null)
            {
                Debug.LogError("Leaderboard data is null.");
                return;
            }

            int loopLength = (msg.Length < names.Count) ? msg.Length : names.Count;

            for (int i = 0; i < loopLength; i++)
            {
                if (names[i] != null && scores[i] != null && rank[i] != null)
                {
                    names[i].color = Color.white;
                    rank[i].color = Color.white;
                    scores[i].color = Color.white;

                    rank[i].text = msg[i].RankSuffix();
                    names[i].text = msg[i].Username;
                    float retrievedTime = msg[i].Score;
                    scores[i].text = (retrievedTime / 1000).ToString("F3");

                    if (msg[i].IsMine())
                    {
                        names[i].color = mineColor;
                        rank[i].color = mineColor;
                        scores[i].color = mineColor;
                    }
                }
                else
                {
                    Debug.LogError($"UI element at index {i} is null.");
                }
            }
        });
    }
    public void GetPersonalEntry(string publicStringKey)
    {
        LeaderboardCreator.GetPersonalEntry(publicStringKey, ((msg) =>
        {
        if (msg.Score != 0)
            {
                mineRank.text = msg.RankSuffix();
                mineName.text = msg.Username;
                float retrievedTime = msg.Score;
                mineScore.text = (retrievedTime / 1000).ToString("F3");

                mineRank.color= mineColor;
                mineName.color= mineColor;
                mineScore.color= mineColor;
            }
            

        }));
    }

    public void SetLeaderboardEntry(string publicStringKey, string username, float time)
    {
        int savedTime = Mathf.RoundToInt(time * 1000f);

        // Fetch the current leaderboard to check the existing time
        LeaderboardCreator.GetLeaderboard(publicStringKey, (leaderboard) =>
        {
            bool entryExists = false;

            foreach (var entry in leaderboard)
            {
                if (entry.Username == username)
                {
                    entryExists = true;
                    if (entry.Score != savedTime)
                    {
                        // Update only if the new time is different
                        LeaderboardCreator.UploadNewEntry(publicStringKey, username, savedTime, (msg) =>
                        {
                            username.Substring(0, 12);
                            GetLeaderboard(publicStringKey);
                        });
                    }
                    else
                    {
                        Debug.Log("Time is the same as the existing one. No update necessary.");
                    }
                    break;
                }
            }

            if (!entryExists)
            {
                // No existing entry, upload a new one
                LeaderboardCreator.UploadNewEntry(publicStringKey, username, savedTime, (msg) =>
                {
                    GetLeaderboard(publicStringKey);
                });
            }
        });
    }
    public void ResetLeaderboard()
    {
        for (int i = 0; i < names.Count; i++)
        {
            names[i].text = "";
            scores[i].text = "";
            rank[i].text = "";

            mineRank.text = "";
            mineName.text = "";
            mineScore.text = "";

            mineRank.color = Color.white;
            mineName.color = Color.white;
            mineScore.color = Color.white;

            names[i].color = Color.white;
            rank[i].color = Color.white;
            scores[i].color = Color.white;
        }
    }
}
