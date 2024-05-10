using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Hetki.Helper;
using System;

public class MatchTypeSelector : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private List<Vector2> cardLayouts;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        cardLayouts = new List<Vector2>();

        //Set Allowed Card Layouts
        cardLayouts.Add(new Vector2(2, 2));
        cardLayouts.Add(new Vector2(2, 3));
        cardLayouts.Add(new Vector2(4, 3));
        cardLayouts.Add(new Vector2(4, 4));
        cardLayouts.Add(new Vector2(5, 6));

        //Setup dropdown
        dropdown.ClearOptions();
        var optionsList = new List<string>();

        foreach (var cardLayout in cardLayouts) 
        {
            optionsList.Add(cardLayout.x+"x"+cardLayout.y);
        }

        dropdown.AddOptions(optionsList);
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        GameManager.GetInstance().SelectedCardLayout = cardLayouts[0];
    }

    private void OnDropdownValueChanged(int index)
    {
        // Set selected layout on value change
        GameManager.GetInstance().SelectedCardLayout = cardLayouts[index];
    }

}
