using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    int amountCards = 0;

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
        boardCards = new List<Card>();
        assignedCards = new HashSet<int>();
        cardTypeAssignments = new Dictionary<int, int>();
        cardDataList = Resources.LoadAll<CardData>("Data/Cards").ToList();
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        cellAdjustor = GetComponent<GridCellAdjustor>();
        //Check required card layout from GameManager
        Vector2 selectedLayout = GameManager.GetInstance().SelectedCardLayout;
        gridLayoutGroup.constraintCount = (int)selectedLayout.x;

        amountCards = (int)(selectedLayout.x * selectedLayout.y);

        //Instantiate and cache cards
        for (int i = 0; i < amountCards; i++)
        {
            GameObject goRef = Instantiate(cardPrefab, transform);
            boardCards.Add(goRef.GetComponent<Card>());
            goRef.GetComponent<Button>().onClick.AddListener(cellAdjustor.RefitGridCells);
        }

        //Assign card pairs
        for (int j = 0;j < amountCards/2; j++) 
        {
            AssignCardPair();
        }
        
    }

    private void Start()
    {
        //Refresh grid UI after setup
        cellAdjustor.RefitGridCells();
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
                        boardCards[cardIndex].SetCardData(cardDataList[cardType]);
                        assignedCards.Add(cardIndex);
                        cardTypeAssignments[cardType]++;
                    }
            }
        }
    }

}
