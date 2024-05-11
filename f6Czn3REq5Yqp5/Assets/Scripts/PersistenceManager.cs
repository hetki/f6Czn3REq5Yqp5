using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Hetki.Helper;
using UnityEngine.SceneManagement;

public class PersistenceManager : MonoBehaviour
{
    private static PersistenceManager Instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        GetInstance();

        Initialization();
    }

    private void Initialization() 
    {
        MonoHelper.Log("noSave: " + PlayerPrefs.GetInt("noSave"));
        if (PlayerPrefs.GetInt("noSave") == -1)
        {
            GameObject.Find("MainMenuManager").GetComponent<MenuNavigation>().LoadGame();
        }
        else
            PlayerPrefs.SetInt("noSave", 0);
    }

    public static PersistenceManager GetInstance()
    {
        if (Instance != null)
            return Instance;
        else
        {
            try
            {
                Instance = GameObject.FindGameObjectWithTag("PersistenceManager").GetComponent<PersistenceManager>();
                return Instance;
            }
            catch
            {
                return null;
            }
        }
    }

    public static MatchState GenerateMatchState(GameManager gameManager, GameObject content) 
    {
        int curSelectedCardIndex = -1;

        if (gameManager.GameBoard.CurSelectedCard != null)
            curSelectedCardIndex = gameManager.GameBoard.CurSelectedCard.transform.GetSiblingIndex();

        MatchState matchState = new MatchState(curSelectedCardIndex, gameManager.ActiveCardsLeft, gameManager.GameBoard.SelectedLayout, null);
        
        matchState.CardStates = new List<CardState>();
        for (int i = 0; i < content.transform.childCount;i++)
        {
            RectTransform rect = content.transform.GetChild(i).GetComponent<RectTransform>();
            Image image = content.transform.GetChild(i).GetComponent<Image>();
            Card card = content.transform.GetChild(i).GetComponent<Card>();

            bool cardShowingFront = false;
            if (gameManager.GameBoard.CurSelectedCard == card && card.ShowingFront)
                cardShowingFront = card.ShowingFront;

            matchState.CardStates.Add(new CardState(new CVector2(rect.anchoredPosition.x, rect.anchoredPosition.y)
                , new CVector2(rect.rect.width, rect.rect.height), new CVector3(rect.localScale.x, rect.localScale.y, rect.localScale.z), image.sprite.name,
                cardShowingFront, card.CardData.id, card.CardData.symbol));
        }

        return matchState;
    }

    public static void SaveMatchState(MatchState matchState)
    {
        string json = JsonConvert.SerializeObject(matchState);
        File.WriteAllText(Application.persistentDataPath + "/matchState.json", json);
    }

    public static MatchState LoadMatchState()
    {
        string path = Application.persistentDataPath + "/matchState.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<MatchState>(json);
        }
        else
        {
            Debug.LogWarning("Save file not found.");
            return null;
        }
    }

    private static void SceneChanged(Scene oldScene, Scene newScene) 
    {
        if(newScene.buildIndex == 0)
            GetInstance().Initialization();
    }

    public static void ClearProgress()
    {
        PlayerPrefs.SetInt("noSave", 1);
        PlayerPrefs.SetInt("turns", 0);
        PlayerPrefs.SetInt("combo", 0);

        SceneManager.activeSceneChanged += SceneChanged;
    }


}
