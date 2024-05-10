using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Hetki.Helper;

public class Card : MonoBehaviour
{
    private bool showingFront = false;
    [SerializeField]
    CardData cardData;
    Button button;
    TMP_Text buttonText;
    Animator animator;
    Sprite frontSprite;
    Sprite backSprite;
    GameBoard gameBoard;

    public CardData CardData
    {
        get { return cardData; }
        set 
        {
            cardData = value;
            buttonText.text = "";
        }
    }
    public bool ShowingFront
    {
        get { return showingFront; }
        set{ showingFront = value; }
    }

    private void Awake()
    {
        gameBoard = transform.parent.GetComponent<GameBoard>();
        button = GetComponent<Button>();
        animator = GetComponent<Animator>();
        buttonText = transform.GetChild(0).GetComponent<TMP_Text>();
        frontSprite = button.image.sprite;
        backSprite = Resources.Load<Sprite>("Sprites/CardBackSprites/CardBack4");
        button.image.sprite = backSprite;
        button.onClick.AddListener(OnClick);
    }

    private void OnClick() 
    {
        if (ShowingFront)
            return; 

        if(!gameBoard.boardLocked)
            FlipCard();
    }

    public void FlipCard()
    {
        if (!ShowingFront)
            animator.SetTrigger("ToFront");
        else
            animator.SetTrigger("ToBack");
    }

    public void ShowCardFront()
    {
        button.image.sprite = frontSprite;
        buttonText.text = CardData.symbol;
        ShowingFront = true;
    }

    public void ShowCardBack()
    {
        buttonText.text = "";
        button.image.sprite = backSprite;
        ShowingFront = false;
    }

}
