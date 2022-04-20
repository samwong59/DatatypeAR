using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDimensionalTextEnvironment : ThreeDimensionalText
{
    [SerializeField]
    private GameObject highlight;

    public int correctList;

    public void ShowHighlight()
    {
        highlight.SetActive(true);
    }

    public void HideHighlight()
    {
        highlight.SetActive(false);
    }
}
