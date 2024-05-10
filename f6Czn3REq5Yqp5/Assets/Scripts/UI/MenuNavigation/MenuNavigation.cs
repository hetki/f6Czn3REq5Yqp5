using Hetki.Helper;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    public void LoadMainMenu()
    {
        StartCoroutine(LoadLevelAsync(0));
    }

    public void LoadNewGame()
    {
        StartCoroutine(LoadLevelAsync(1));
    }

    IEnumerator LoadLevelAsync(int level)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            MonoHelper.Log("Scene Load Progress: " + asyncOperation.progress);
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    public void HideMenu(GameObject menu)
    {
        menu.SetActive(false);
    }

    public void ShowMenu(GameObject menu)
    {
        menu.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
