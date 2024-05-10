using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Hetki.Helper;

public class GameManager : MonoBehaviour
{
    TMP_Text coinsTmp;
    TMP_Text multiplierTmp;
    GameBoard gameBoard;

    int coins = 0;
    int activeCardsLeft = 0;
    int multiplier = 0;

    public int Coins
    {
        get { return coins; }
        set 
        {
            coins = value;
            if(coinsTmp)
                coinsTmp.text = coins.ToString();
        }
    }

    public int ActiveCardsLeft
    {
        get { return activeCardsLeft; }
        set { activeCardsLeft = value; }
    }

    public int Multiplier
    {
        get { return multiplier; }
        set 
        { 
            multiplier = value;
            if (multiplierTmp)
                multiplierTmp.text = "Multiplier: " + multiplier.ToString();
        }
    }

    private void Awake()
    {
        gameBoard = transform.Find("Content").GetComponent<GameBoard>();
        coinsTmp = transform.Find("CoinsTxt").GetComponent<TMP_Text>();
        multiplierTmp = transform.Find("MultiplierTxt").GetComponent<TMP_Text>();

        Coins = PlayerPrefs.GetInt("coins");
        coinsTmp.text = Coins.ToString();
        multiplierTmp.text = Multiplier.ToString();
        ResetMultiplier();
    }

    public void IncreaseMultiplier() 
    {
        Multiplier++;
    }

    public void ResetMultiplier() 
    {
        Multiplier = 1;
    }


    public void GameOver()
    {
        SoundManager.GetInstance().PlaySound(Sounds.GameOver);
        SaveProgress();
        RestartMatch();
        MonoHelper.Log("Game Over");
    }

    public void RestartMatch() 
    {
        StartCoroutine(DelayedBoardReset(1.5f));
    }

    IEnumerator DelayedBoardReset(float delay) 
    {
        yield return MonoHelper.GetWait(delay);
        gameBoard.ResetBoard();
    }

    private void SaveProgress() 
    {
        PlayerPrefs.SetInt("coins", Coins);
    }

}
