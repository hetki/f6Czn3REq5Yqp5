using System;
using System.Collections.Generic;

/// <summary>
/// MatchState for persistence system
/// </summary>
[Serializable]
public class MatchState
{
    private int curSelectedCardIndex;
    private int activeCardsLeft;
    private CVector2 cardLayout;
    private List<CardState> cardStates;

    public int CurSelectedCardIndex
    {
        get { return curSelectedCardIndex; }
        set { curSelectedCardIndex = value; }
    }

    public int ActiveCardsLeft
    {
        get { return activeCardsLeft; }
        set { activeCardsLeft = value; }
    }

    public CVector2 CardLayout
    {
        get { return cardLayout; }
        set { cardLayout = value; }
    }

    public List<CardState> CardStates
    {
        get { return cardStates; }
        set { cardStates = value; }
    }

    public MatchState(int curSelectedCardIndex, int activeCardsLeft, CVector2 cardLayout, List<CardState> cardStates)
    {
        this.curSelectedCardIndex = curSelectedCardIndex;
        this.activeCardsLeft = activeCardsLeft;
        this.cardLayout = cardLayout;
        this.cardStates = cardStates;
    }
}

/// <summary>
/// CardStates for persistence system
/// </summary>
[Serializable]
public class CardState 
{
    private CVector2 position;
    private CVector2 size;
    private CVector3 scale;
    private string spriteName;
    private bool cardShowingFront;
    private int cardId;
    private string cardSymbol;

    public CVector2 Position
    {
        get { return position; }
        set { position = value; }
    }

    public CVector2 Size
    {
        get { return size; }
        set { size = value; }
    }

    public CVector3 Scale
    {
        get { return scale; }
        set { scale = value; }
    }

    public string SpriteName
    {
        get { return spriteName; }
        set { spriteName = value; }
    }

    public bool CardShowingFront
    {
        get { return cardShowingFront; }
        set { cardShowingFront = value; }
    }

    public int CardId
    {
        get { return cardId; }
        set { cardId = value; }
    }

    public string CardSymbol
    {
        get { return cardSymbol; }
        set { cardSymbol = value; }
    }

    public CardState(CVector2 position, CVector2 size, CVector3 scale, string spriteName,bool cardShowingFront, int cardId, string cardSymbol)
    {
        this.position = position;
        this.size = size;
        this.scale = scale;
        this.spriteName = spriteName;
        this.cardShowingFront = cardShowingFront;
        this.cardId = cardId;
        this.cardSymbol = cardSymbol;
    }
}
