using Hetki.Helper;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// MenuNavigation methods to attach to menu buttons
/// </summary>
public class MenuNavigation : MonoBehaviour
{
    /// <summary>
    /// SoundManagerPrefab - Only assign in main scene
    /// </summary>
    [SerializeField]
    GameObject soundManagerPrefab;
    /// <summary>
    /// PersistenceManagerPrefab - Only assign in main scene
    /// </summary>
    [SerializeField]
    GameObject persistenceManagerPrefab;

    /// <summary>
    /// Initialization
    /// </summary>
    private void Awake()
    {
        //Setup singletons from here, for use in main menu scene
        if(soundManagerPrefab)
            if (SoundManager.GetInstance() == null)
                Instantiate(soundManagerPrefab);
        
        if (persistenceManagerPrefab)
            if (PersistenceManager.GetInstance() == null)
                Instantiate(persistenceManagerPrefab);
    }

    /// <summary>
    /// Load main menu scene async
    /// </summary>
    public void LoadMainMenu()
    {
        PersistenceManager.ClearProgress();
        StartCoroutine(LoadLevelAsync(0));
    }

    /// <summary>
    /// Load game scene async
    /// </summary>
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
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Hide menu
    /// </summary>
    /// <param name="menu"></param>
    public void HideMenu(GameObject menu)
    {
        menu.SetActive(false);
    }

    /// <summary>
    /// Show menu
    /// </summary>
    /// <param name="menu"></param>
    public void ShowMenu(GameObject menu)
    {
        menu.SetActive(true);
    }

    /// <summary>
    /// Save and exit game
    /// </summary>
    public void SaveExit()
    {
        GameManager gameManager = transform.Find("PlayArea").GetComponent<GameManager>();

        //Only save if there's cards left
        if(gameManager.ActiveCardsLeft > 0) 
        {
            //Mark as has save
            PlayerPrefs.SetInt("noSave", -1);
            //Only meant for in-game where game manager would be present
            gameManager.SaveProgress();
        }
        else
            PersistenceManager.ClearProgress();

        Application.Quit();
    }

    /// <summary>
    /// Exit game without saving
    /// </summary>
    public void Exit()
    {
        PersistenceManager.ClearProgress();
        Application.Quit();
    }

}
