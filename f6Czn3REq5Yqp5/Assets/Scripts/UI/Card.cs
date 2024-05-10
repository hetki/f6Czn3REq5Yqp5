using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card : MonoBehaviour
{
    [SerializeField]
    CardData cardData;
    Button button;
    TMP_Text buttonText;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void SetCardData(CardData newData) 
    {
        cardData = newData;
        buttonText.text = newData.symbol;
    }
}
