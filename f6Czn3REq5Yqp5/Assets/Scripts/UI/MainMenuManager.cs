using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text coinsText;

    private void Start()
    {
        SetCoinsText(PlayerPrefs.GetInt("coins"));
    }

    private void SetCoinsText(int coins) 
    {
        coinsText.text = "Coins: " + coins;
    }

}
