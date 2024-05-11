using Hetki.Helper;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    [SerializeField]
    GameObject soundManagerPrefab;
    [SerializeField]
    GameObject persistenceManagerPrefab;

    private void Awake()
    {
        //Setup singletons
        if(soundManagerPrefab)
            if (SoundManager.GetInstance() == null)
                Instantiate(soundManagerPrefab);
        
        if (persistenceManagerPrefab)
            if (PersistenceManager.GetInstance() == null)
                Instantiate(persistenceManagerPrefab);
    }

    public void LoadMainMenu()
    {
        PersistenceManager.ClearProgress();
        StartCoroutine(LoadLevelAsync(0));
    }

    public void LoadGame()
    {
        MonoHelper.Log("LOAD GAME");
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

    public void SaveExit()
    {
        //Mark as has save
        PlayerPrefs.SetInt("noSave", -1);
        //Only meant for in-game where game manager would be present
        transform.Find("PlayArea").GetComponent<GameManager>().SaveProgress();
        Application.Quit();
    }

    public void Exit()
    {
        PersistenceManager.ClearProgress();
        Application.Quit();
    }

}
