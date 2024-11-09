using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using System.Threading;

public class Timing : MonoBehaviour
{
    public TextMeshProUGUI timer, countdown, status;
    public float timing = 0f;

    public bool timerActive;
    public float savedTime;

    bool startCount;
    int startCountdownInt;



    // Update is called once per frame
    void Update()
    {
        if(timerActive)
        {
            TimerActive();
        }
        timer.text = timing.ToString("F3");
    }

    bool countdownActive;

    public IEnumerator StartCountdown()
    {
        GameManager.Instance.waitingForCountdown = true;

        if (!GameManager.Instance.waitingForCountdown)
            yield break; // Don't start a new countdown if one is already active

        GameManager.Instance.countdownPage.SetActive(true);
        startCountdownInt = 3; // Use the duration passed in

        status.text = "Get Ready";

        while (startCountdownInt >= 1)
        {
            Debug.Log(startCountdownInt);
            countdown.text = startCountdownInt.ToString();
            yield return new WaitForSeconds(.3f);
            startCountdownInt--;
        }

        timerActive = true;

        // Re-enable all input actions
        Debug.Log("Enabling inputs");
        //UserInputs.inputREF.actions.Enable();

        GameManager.Instance.waitingForCountdown = false; // Reset the flag when countdown is done

        GameManager.Instance.countdownPage.SetActive(false);
        GameManager.Instance.gameUI.SetActive(true);



        Debug.Log("Finished");
    }

    void TimerActive()
    {
        timing += Time.deltaTime;
    }

    public void SaveTime()
    {
        savedTime = timing;
        if (GameManager.Instance.level.allTimes.Count < 5f)
        {
            // If we have less than maxTimes, just add the new time
            GameManager.Instance.level.allTimes.Add(savedTime);
        }
        else
        {
            // Find the slowest time in the list
            float slowestTime = GameManager.Instance.level.allTimes[0];
            int slowestIndex = 0;

            for (int i = 1; i < GameManager.Instance.level.allTimes.Count; i++)
            {
                if (GameManager.Instance.level.allTimes[i] > slowestTime)
                {
                    slowestTime = GameManager.Instance.level.allTimes[i];
                    slowestIndex = i;
                }
            }

            // If the new time is faster than the slowest time, replace it
            if (savedTime < slowestTime)
            {
                GameManager.Instance.level.allTimes[slowestIndex] = savedTime;
            }
        }


        // Sort the list to keep it ordered from fastest to slowest
        GameManager.Instance.level.allTimes.Sort();

        GameManager.Instance.level.bestTime = GameManager.Instance.level.allTimes[0];
    }
}
