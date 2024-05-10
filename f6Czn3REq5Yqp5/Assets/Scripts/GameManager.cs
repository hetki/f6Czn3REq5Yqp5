using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Hetki.Helper;
using Unity.Burst.Intrinsics;
using UnityEngine.UI;

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

    private void Awake()
    {
        restartButton = transform.Find("RestartBtn").GetComponent<Button>();
        restartButton.gameObject.SetActive(false);
        gameBoard = transform.Find("Content").GetComponent<GameBoard>();
        turnsTmp = transform.Find("TurnsTxt").GetComponent<TMP_Text>();
        comboTmp = transform.Find("ComboTxt").GetComponent<TMP_Text>();

        Turns = PlayerPrefs.GetInt("turns");
        Combo = PlayerPrefs.GetInt("combo");

        ResetCombo();
    }

    public void IncreaseCombo() 
    {
        Combo++;
    }

    public void ResetCombo() 
    {
        Combo = 0;
    }

    public void GameOver()
    {
        SoundManager.GetInstance().PlaySound(Sounds.GameOver);
        StartCoroutine(DelayedShowResetButton(1.3f));
        ClearProgress();
        MonoHelper.Log("Game Over");
    }

    public void RestartMatch() 
    {
        Turns = 0;
        Combo = 0;
        restartButton.gameObject.SetActive(false);
        StartCoroutine(DelayedBoardReset(1f));
    }

    IEnumerator DelayedShowResetButton(float delay)
    {
        yield return MonoHelper.GetWait(delay);
        restartButton.gameObject.SetActive(true);
    }

    IEnumerator DelayedBoardReset(float delay) 
    {
        yield return MonoHelper.GetWait(delay);
        gameBoard.ResetBoard();
    }

    private void ClearProgress() 
    {
        PlayerPrefs.SetInt("turns", 0);
        PlayerPrefs.SetInt("combo", 0);
    }

    private void SaveProgress() 
    {
        PlayerPrefs.SetInt("turns", Turns);
        PlayerPrefs.SetInt("combo", Combo);
    }

}
