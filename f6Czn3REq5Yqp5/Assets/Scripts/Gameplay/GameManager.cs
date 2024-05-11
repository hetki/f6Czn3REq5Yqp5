using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Hetki.Helper;
using Unity.Burst.Intrinsics;
using UnityEngine.UI;

/// <summary>
/// Gameplay management in Game Scene
/// </summary>
public class GameManager : MonoBehaviour
{
    TMP_Text turnsTmp;
    TMP_Text comboTmp;
    GameBoard gameBoard;
    Button restartButton;

    int turns = 0;
    int activeCardsLeft = 0;
    int combo = 0;

    public int Turns
    {
        get { return turns; }
        set 
        {
            turns = value;
            if(turnsTmp)
                turnsTmp.text = "Turns: " + turns.ToString();
        }
    }

    public int ActiveCardsLeft
    {
        get { return activeCardsLeft; }
        set { activeCardsLeft = value; }
    }

    public int Combo
    {
        get { return combo; }
        set 
        {
            combo = value;
            if (comboTmp)
                comboTmp.text = "Combo: X" + Combo.ToString();
        }
    }
    public GameBoard GameBoard
    {
        get { return gameBoard; }
        set { gameBoard = value; }
    }

    /// <summary>
    /// Do GameManager initialization
    /// </summary>
    private void Awake()
    {
        restartButton = transform.Find("RestartBtn").GetComponent<Button>();
        restartButton.gameObject.SetActive(false);
        gameBoard = transform.Find("Content").GetComponent<GameBoard>();
        turnsTmp = transform.Find("TurnsTxt").GetComponent<TMP_Text>();
        comboTmp = transform.Find("ComboTxt").GetComponent<TMP_Text>();

        Turns = PlayerPrefs.GetInt("turns");
        Combo = PlayerPrefs.GetInt("combo");
    }

    /// <summary>
    /// Increases combo +1
    /// </summary>
    public void IncreaseCombo() 
    {
        Combo++;
    }

    /// <summary>
    /// Sets combo to 0
    /// </summary>
    public void ResetCombo() 
    {
        Combo = 0;
    }

    /// <summary>
    /// Sets game state as "Over", resets board and progress
    /// </summary>
    public void GameOver()
    {
        SoundManager.GetInstance().PlaySound(Sounds.GameOver);
        StartCoroutine(DelayedShowResetButton(1.3f));
        PersistenceManager.ClearProgress();
    }

    /// <summary>
    /// Reset match variables and show new board
    /// </summary>
    public void RestartMatch()
    {
        Turns = 0;
        Combo = 0;
        restartButton.gameObject.SetActive(false);
        StartCoroutine(DelayedBoardReset(1f));
    }

    /// <summary>
    /// Reset board with delay
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator DelayedBoardReset(float delay)
    {
        yield return MonoHelper.GetWait(delay);
        gameBoard.ResetBoard();
    }

    /// <summary>
    /// Show reset button with delay
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator DelayedShowResetButton(float delay)
    {
        yield return MonoHelper.GetWait(delay);
        restartButton.gameObject.SetActive(true);
    }

    /// <summary>
    /// Save game progress with persistence system when flagged
    /// </summary>
    public void SaveProgress() 
    {
        if (PlayerPrefs.GetInt("noSave") != 1) 
        {
            PlayerPrefs.SetInt("noSave", -1);
            PlayerPrefs.SetInt("turns", Turns);
            PlayerPrefs.SetInt("combo", Combo);

            MatchState matchState = PersistenceManager.GenerateMatchState(this, transform.Find("Content").gameObject);
            PersistenceManager.SaveMatchState(matchState);
        }
    }
}
