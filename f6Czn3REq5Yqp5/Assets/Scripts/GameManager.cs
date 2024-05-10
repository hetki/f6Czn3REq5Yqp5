using Hetki.Helper;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField]
    private int coins;
    [SerializeField]
    private Vector2 selectedCardLayout;

    public int Coins
    {
        get { return coins; }
        set { coins = value; }
    }

    public Vector2 SelectedCardLayout
    {
        get { return selectedCardLayout; }
        set { selectedCardLayout = value; }
    }

    public static GameManager GetInstance()
    {
        if (Instance != null)
            return Instance;
        else
        {
            try
            {
                Instance = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
                Instance.Initialize();
                return Instance;
            }
            catch (System.Exception ex)
            {
                //Could not find GO
                MonoHelper.LogError("GetInstance() Error: " + ex.Message + "\n" + ex.StackTrace);
                return null;
            }
        }
    }

    private void Initialize()
    {
        coins = PlayerPrefs.GetInt("coins", 0);

        //Keep GameManager 'alive' for persistence
        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        GetInstance();
    }

}
