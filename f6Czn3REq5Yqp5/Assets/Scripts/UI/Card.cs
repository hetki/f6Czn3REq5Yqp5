using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Hetki.Helper;

/// <summary>
/// Card behaviour
/// </summary>
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

    /// <summary>
    /// Card initialization
    /// </summary>
    private void Awake()
    {
        //Set card's required variables
        gameBoard = transform.parent.GetComponent<GameBoard>();
        button = GetComponent<Button>();
        animator = GetComponent<Animator>();
        buttonText = transform.GetChild(0).GetComponent<TMP_Text>();
        frontSprite = button.image.sprite;
        //Set default sprite as card back side
        backSprite = Resources.Load<Sprite>("Sprites/CardBackSprites/CardBack9");
        button.image.sprite = backSprite;
        button.onClick.AddListener(OnClick);
    }

    /// <summary>
    /// Card on click event
    /// </summary>
    private void OnClick() 
    {
        if (ShowingFront)
            return; 

        if(!gameBoard.BoardLocked)
            FlipCard();
    }

    /// <summary>
    /// Flip card with animator
    /// </summary>
    public void FlipCard()
    {
        //Only flip to front when back is displayed
        if (!ShowingFront)
            animator.SetTrigger("ToFront");
        else
            animator.SetTrigger("ToBack");
    }

    /// <summary>
    /// Show card front side
    /// </summary>
    public void ShowCardFront()
    {
        button.image.sprite = frontSprite;
        buttonText.text = CardData.symbol;
        ShowingFront = true;
    }

    /// <summary>
    /// Show card back side
    /// </summary>
    public void ShowCardBack()
    {
        buttonText.text = "";
        button.image.sprite = backSprite;
        ShowingFront = false;
    }

}
