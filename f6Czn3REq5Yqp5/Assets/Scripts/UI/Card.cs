using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    [SerializeField]
    CardData cardData;
    Button button;
    TMP_Text buttonText;

    public CardData CardData
    {
        get { return cardData; }
        set 
        {
            cardData = value;
            buttonText.text = value.symbol;
        }
    }

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = transform.GetChild(0).GetComponent<TMP_Text>();
    }

}
