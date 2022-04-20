using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ThreeDimensionalText : MonoBehaviour
{

    TMP_Text[] textChildren;

    public void ChangeText(string newText)
    {
        if (textChildren == null)
        {
            textChildren = GetComponentsInChildren<TMP_Text>();
        }
        foreach(TMP_Text textChild in textChildren)
        {
            textChild.text = newText;
        }
    }

}
