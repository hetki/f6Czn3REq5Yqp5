using Hetki.Helper;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Game board logic
/// </summary>
public class GameBoard : MonoBehaviour
{
    private bool boardLocked = true;
    private int amountCards = 0;

    private Card previousSelectedCard;
    private Card curSelectedCard;

    private GameManager gameManager;
    [SerializeField]
    private GameObject cardPrefab;
    private GridLayoutGroup gridLayoutGroup;
    private GridCellAdjustor cellAdjustor;
    private List<CardData> cardDataList;
    private List<Card> boardCards;
    private CVector2 selectedLayout;

    private HashSet<int> assignedCards;
    private Dictionary<int, int> cardTypeAssignments;

    public Card CurSelectedCard
    {
        get { return curSelectedCard; }
        set { curSelectedCard = value; }
    }

    public bool BoardLocked
    {
        get { return boardLocked; }
        set { boardLocked = value; }
    }

    public CVector2 SelectedLayout
    {
        get { return selectedLayout; }
        set { selectedLayout = value; }
    }

    /// <summary>
    /// Do initialization for board
    /// </summary>
    private void Awake()
    {
        gameManager = transform.parent.GetComponent<GameManager>();
        boardCards = new List<Card>();
        assignedCards = new HashSet<int>();
        cardTypeAssignments = new Dictionary<int, int>();
        cardDataList = Resources.LoadAll<CardData>("Data/Cards").ToList();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        cellAdjustor = GetComponent<GridCellAdjustor>();

        //If no save, continue normally
        if(PlayerPrefs.GetInt("noSave") != -1)
            ResetBoard();
        //Otherwise start restoring previous save
        else 
        {
            RestoreState();
        }

    }

    /// <summary>
    /// Delayed allowal of interaction
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator UnlockBoard(float delay)
    {
        yield return MonoHelper.GetWait(delay);
        boardLocked = false;
    }

    /// <summary>
    /// Flip all cards with delay
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator FlipAllCards(float delay)
    {
        yield return MonoHelper.GetWait(delay);
        foreach (var card in boardCards)
        {
            card.FlipCard();
        }
    }

    /// <summary>
    /// Check current selection on button click
    /// </summary>
    /// <param name="card"></param>
    void CheckSelection(Card card) 
    {
        if (card.ShowingFront)
            return;

        if (boardLocked)
            return;

        SoundManager.GetInstance().PlaySound(Sounds.CardFlip);

        //Set history
        if(curSelectedCard != null)
            previousSelectedCard = curSelectedCard;

        //Set new current card
        curSelectedCard = card;

        if (previousSelectedCard != null) 
        {
            //Same type but not same card
            if (previousSelectedCard.CardData.id == curSelectedCard.CardData.id &&
                curSelectedCard.GetInstanceID() != previousSelectedCard.GetInstanceID())
            {
                //Match!
                CardsMatched();
            }
            //Not same type
            else if (previousSelectedCard.CardData.id != curSelectedCard.CardData.id)
            {
                //Mismatch!
                CardsMismatched();
            }
        }

        //Game over when no active cards left
        if (gameManager.ActiveCardsLeft == 0)
            gameManager.GameOver();
    }

    /// <summary>
    /// Sequence to run when cards match
    /// </summary>
    void CardsMatched() 
    {
        StartCoroutine(DelayedCardsRemoval(previousSelectedCard, curSelectedCard));
        ResetSelectedCards();
        gameManager.ActiveCardsLeft -= 2;
        gameManager.IncreaseCombo();
        gameManager.Turns += 1;

        SoundManager.GetInstance().PlaySound(Sounds.Match);
    }

    /// <summary>
    /// Sequence to run when cards mismatch
    /// </summary>
    void CardsMismatched()
    {
        if(curSelectedCard != previousSelectedCard)
            StartCoroutine(DelayedCardsReset(previousSelectedCard, curSelectedCard));
        else
            StartCoroutine(DelayedCardReset(curSelectedCard));
        ResetSelectedCards();
        gameManager.ResetCombo();
        gameManager.Turns += 1;

        SoundManager.GetInstance().PlaySound(Sounds.Mismatch);
    }

    /// <summary>
    /// Remove cards with delay
    /// </summary>
    /// <param name="card1"></param>
    /// <param name="card2"></param>
    /// <returns></returns>
    IEnumerator DelayedCardsRemoval(Card card1, Card card2)
    {
        yield return MonoHelper.GetWait(1f);
        Destroy(card1.gameObject);
        Destroy(card2.gameObject);
    }

    /// <summary>
    /// Reset card with delay
    /// </summary>
    /// <param name="card1"></param>
    /// <returns></returns>
    IEnumerator DelayedCardReset(Card card1)
    {
        yield return MonoHelper.GetWait(1f);
        card1.FlipCard();
    }

    /// <summary>
    /// Reset cards with delay
    /// </summary>
    /// <param name="card1"></param>
    /// <param name="card2"></param>
    /// <returns></returns>
    IEnumerator DelayedCardsReset(Card card1, Card card2) 
    {
        yield return MonoHelper.GetWait(1f);
        card1.FlipCard();
        card2.FlipCard();
    }

    /// <summary>
    /// Reset selected cards
    /// </summary>
    void ResetSelectedCards() 
    {
        previousSelectedCard = null;
        curSelectedCard = null;
    }

    /// <summary>
    /// Determine what card pair type isn't in use and pick it
    /// </summary>
    void AssignCardPair() 
    {
        //Pick card type - must have 0 assignments
        int cardType = Random.Range(0, cardDataList.Count);
        while (cardTypeAssignments.ContainsKey(cardType)) 
        {
            cardType = Random.Range(0, cardDataList.Count);
        }

        cardTypeAssignments.Add(cardType,0);

        //Ensure we're getting 2 cards
        while (cardTypeAssignments[cardType] < 2) 
        {
            int cardIndex = Random.Range(0, boardCards.Count);
            if (!assignedCards.Contains(cardIndex)) 
            {
                if(cardTypeAssignments.ContainsKey(cardType))
                    if (cardTypeAssignments[cardType] < 2) 
                    {
                        //Assign card
                        boardCards[cardIndex].CardData = cardDataList[cardType];
                        assignedCards.Add(cardIndex);
                        cardTypeAssignments[cardType]++;
                    }
            }
        }
    }

    /// <summary>
    /// Reset the game board
    /// </summary>
    public void ResetBoard() 
    {
        boardLocked = true;

        //Remove children when reset is triggered elsewhere
        if(transform.childCount > 0)
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(0).gameObject);
            }

        boardCards.Clear();
        assignedCards.Clear();
        cardTypeAssignments.Clear();

        //Check required card layout from GameManager
        selectedLayout = MonoHelper.StringToCardLayout(PlayerPrefs.GetString("cardLayout"));
        gridLayoutGroup.constraintCount = (int)selectedLayout.x;

        amountCards = (int)(selectedLayout.x * selectedLayout.y);
        gameManager.ActiveCardsLeft = amountCards;

        //Instantiate and cache cards
        for (int i = 0; i < amountCards; i++)
        {
            GameObject goRef = Instantiate(cardPrefab, transform);
            Card card = goRef.GetComponent<Card>();
            boardCards.Add(card);
            Button buttonRef = goRef.GetComponent<Button>();
            buttonRef.onClick.AddListener(() => CheckSelection(card));
        }

        //Assign card pairs
        for (int j = 0; j < amountCards / 2; j++)
        {
            AssignCardPair();
        }

        cellAdjustor.ResetGridLayout();
        StartCoroutine(FlipAllCards(1f));
        StartCoroutine(FlipAllCards(3f));
        StartCoroutine(UnlockBoard(3.5f));
    }

    /// <summary>
    /// Restore game with MatchState
    /// </summary>
    private void RestoreState()
    {
        //Load old matchState
        MatchState matchState = PersistenceManager.LoadMatchState();

        //Assign active gameplay vars
        selectedLayout = matchState.CardLayout;
        amountCards = matchState.ActiveCardsLeft;
        gameManager.ActiveCardsLeft = matchState.ActiveCardsLeft;

        //Keep stored card positions
        gridLayoutGroup.enabled = false;
        //Instantiate, set and cache cards
        for (int i = 0; i < amountCards; i++)
        {
            GameObject goRef = Instantiate(cardPrefab, transform);
            RectTransform rect = goRef.GetComponent<RectTransform>();

            rect.anchoredPosition = (Vector2)matchState.CardStates[i].Position;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.sizeDelta = (Vector2)matchState.CardStates[i].Size;
            rect.localScale = (Vector3)matchState.CardStates[i].Scale;

            Card card = goRef.GetComponent<Card>();
            CardData cardData = ScriptableObject.CreateInstance<CardData>();
            cardData.id = matchState.CardStates[i].CardId;
            cardData.symbol = matchState.CardStates[i].CardSymbol;

            card.CardData = cardData;

            if (matchState.CardStates[i].CardShowingFront)
                card.FlipCard();

            boardCards.Add(card);
            Button buttonRef = goRef.GetComponent<Button>();
            buttonRef.onClick.AddListener(() => CheckSelection(card));
        }

        //CurSelectedCard Persistence
        if (matchState.CurSelectedCardIndex != -1)
            curSelectedCard = boardCards[matchState.CurSelectedCardIndex];

        StartCoroutine(UnlockBoard(.5f));
    }

}
