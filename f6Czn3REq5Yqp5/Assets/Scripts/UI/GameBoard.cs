using Hetki.Helper;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
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

    HashSet<int> assignedCards;
    Dictionary<int, int> cardTypeAssignments;

    private void Awake()
    {
        gameManager = transform.parent.GetComponent<GameManager>();
        boardCards = new List<Card>();
        assignedCards = new HashSet<int>();
        cardTypeAssignments = new Dictionary<int, int>();
        cardDataList = Resources.LoadAll<CardData>("Data/Cards").ToList();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        cellAdjustor = GetComponent<GridCellAdjustor>();

        ResetBoard();

    }

    private void Start()
    {
        //Refresh grid UI after setup
        cellAdjustor.RefitGridCells();
        StartCoroutine(DisableGridLayout());
    }

    IEnumerator DisableGridLayout()
    {
        yield return new WaitForEndOfFrame();
        gridLayoutGroup.enabled = false;
    }

    void CheckSelection(Card card) 
    {

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
            else if (previousSelectedCard.CardData.id != curSelectedCard.CardData.id ||
                curSelectedCard.GetInstanceID() == previousSelectedCard.GetInstanceID())
            {
                //Mismatch!
                CardsMismatched();
            }
        }

        if (gameManager.ActiveCardsLeft == 0)
            gameManager.GameOver();
    }

    void CardsMatched() 
    {
        previousSelectedCard.gameObject.SetActive(false);
        curSelectedCard.gameObject.SetActive(false);
        ResetCards();
        gameManager.ActiveCardsLeft -= 2;
        gameManager.IncreaseMultiplier();
        gameManager.Coins += 20 * gameManager.Multiplier;

        SoundManager.GetInstance().PlaySound(Sounds.Match);
    }

    void CardsMismatched()
    {
        MonoHelper.Log("Mismatch!");
        ResetCards();
        gameManager.ResetMultiplier();

        SoundManager.GetInstance().PlaySound(Sounds.Mismatch);
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
        gridLayoutGroup.enabled = true;

        boardCards.Clear();
        assignedCards.Clear();
        cardTypeAssignments.Clear();

        //Check required card layout from GameManager
        Vector2 selectedLayout = MonoHelper.StringToCardLayout(PlayerPrefs.GetString("cardLayout"));
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


    }

}
