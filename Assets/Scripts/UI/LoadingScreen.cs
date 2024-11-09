using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar;
    public GameObject loadingScreen;


    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsync(sceneIndex));
    }
    public IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        // Prevent the scene from activating immediately
        operation.allowSceneActivation = false;

        // Activate the loading screen
        loadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            // Calculate the progress (0.0 to 1.0)
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progressValue;

            // Check if the load is complete (almost 0.9)
            if (operation.progress >= 0.9f)
            {

                // Allow the scene to activate
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
