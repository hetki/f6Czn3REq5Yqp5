using Hetki.Helper;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    bool boardLocked = true;
    int amountCards = 0;

    Card previousSelectedCard;
    Card curSelectedCard;

    GameManager gameManager;
    [SerializeField]
    GameObject cardPrefab;
    GridLayoutGroup gridLayoutGroup;
    GridCellAdjustor cellAdjustor;
    List<CardData> cardDataList;
    List<Card> boardCards;
    CVector2 selectedLayout;

    HashSet<int> assignedCards;
    Dictionary<int, int> cardTypeAssignments;

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

    private void Awake()
    {
        gameManager = transform.parent.GetComponent<GameManager>();
        boardCards = new List<Card>();
        assignedCards = new HashSet<int>();
        cardTypeAssignments = new Dictionary<int, int>();
        cardDataList = Resources.LoadAll<CardData>("Data/Cards").ToList();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        cellAdjustor = GetComponent<GridCellAdjustor>();

        if(PlayerPrefs.GetInt("noSave") != -1)
            ResetBoard();
        else 
        {
            RestoreState();
        }

    }

    IEnumerator UnlockBoard(float delay)
    {
        yield return MonoHelper.GetWait(delay);
        boardLocked = false;
    }

    IEnumerator FlipAllCards(float delay)
    {
        yield return MonoHelper.GetWait(delay);
        foreach (var card in boardCards)
        {
            card.FlipCard();
        }
    }

    void CheckSelection(Card card) 
    {
        if (card.ShowingFront)
            return;

        if (boardLocked)
            return;

        MonoHelper.Log("ID: " + card.CardData.id);
        SoundManager.GetInstance().PlaySound(Sounds.CardFlip);

        if(curSelectedCard != null)
            previousSelectedCard = curSelectedCard;

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
            //Not same code or same card
            else if (previousSelectedCard.CardData.id != curSelectedCard.CardData.id)
            {
                //Mismatch!
                CardsMismatched();
            }
            else if (curSelectedCard.GetInstanceID() == previousSelectedCard.GetInstanceID()) 
            {
                ResetCards();
            }
        }

        if (gameManager.ActiveCardsLeft == 0)
            gameManager.GameOver();
    }

    void CardsMatched() 
    {
        StartCoroutine(DelayedCardsRemoval(previousSelectedCard, curSelectedCard));
        ResetCards();
        gameManager.ActiveCardsLeft -= 2;
        gameManager.IncreaseCombo();
        gameManager.Turns += 1;

        SoundManager.GetInstance().PlaySound(Sounds.Match);
    }

    void CardsMismatched()
    {
        MonoHelper.Log("Mismatch!");
        if(curSelectedCard != previousSelectedCard)
            StartCoroutine(DelayedCardsReset(previousSelectedCard, curSelectedCard));
        else
            StartCoroutine(DelayedCardReset(curSelectedCard));
        ResetCards();
        gameManager.ResetCombo();
        gameManager.Turns += 1;

        SoundManager.GetInstance().PlaySound(Sounds.Mismatch);
    }

    IEnumerator DelayedCardsRemoval(Card card1, Card card2)
    {
        yield return MonoHelper.GetWait(1f);
        Destroy(card1.gameObject);
        Destroy(card2.gameObject);
    }

    IEnumerator DelayedCardReset(Card card1)
    {
        yield return MonoHelper.GetWait(1f);
        card1.FlipCard();
    }

    IEnumerator DelayedCardsReset(Card card1, Card card2) 
    {
        yield return MonoHelper.GetWait(1f);
        card1.FlipCard();
        card2.FlipCard();
    }

    void ResetCards() 
    {
        previousSelectedCard = null;
        curSelectedCard = null;
    }

    void AssignCardPair() 
    {
        //Pick card type - must have 0 assignments
        int cardType = Random.Range(0, cardDataList.Count);
        while (cardTypeAssignments.ContainsKey(cardType)) 
        {
            cardType = Random.Range(0, cardDataList.Count);
        }

        cardTypeAssignments.Add(cardType,0);

        //Ensure we assing 2 cards
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

    public void ResetBoard() 
    {
        MonoHelper.Log("ResetBoard");
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
        StartCoroutine(UnlockBoard(4.25f));
    }

    private void RestoreState()
    {
        //Load old matchState
        MatchState matchState = PersistenceManager.LoadMatchState();

        selectedLayout = matchState.CardLayout;
        amountCards = matchState.ActiveCardsLeft;
        gameManager.ActiveCardsLeft = matchState.ActiveCardsLeft;

        gridLayoutGroup.enabled = false;
        //Instantiate and cache cards
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

        if (matchState.CurSelectedCardIndex != -1)
            curSelectedCard = boardCards[matchState.CurSelectedCardIndex];

        StartCoroutine(UnlockBoard(1f));

    }

}
