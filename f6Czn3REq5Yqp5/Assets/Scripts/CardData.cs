using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "New Card", menuName = "Hetki/Data/Card")]
public class CardData : ScriptableObject
{

    public int id;
    public string symbol;
}