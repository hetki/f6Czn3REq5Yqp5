using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Hetki/Data/Card")]
public class CardData : ScriptableObject
{
    public int id;
    public string symbol;

    public CardData(int id, string symbol)
    {
        this.id = id;
        this.symbol = symbol;
    }
}