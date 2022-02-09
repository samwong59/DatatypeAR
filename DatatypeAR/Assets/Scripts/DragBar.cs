using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class DragBar : MonoBehaviour
{
    [SerializeField]
    TMP_Text goldBarText;
    private List<Value> availableValues = new List<Value>();
    private List<Value> usedValues = new List<Value>();
    private System.Random random = new System.Random();
    public Value currentValue;

    public class Value
    {
        string dataType;
        string value;

        public Value(string value, string datatype)
        {
            this.value = value;
            this.dataType = datatype;
        }

        public string getValue()
        {
            return value;
        }

        public string getDataType()
        {
            return dataType;
        }
    }


    public void InitialValue(string level)
    {
        CreateValues(level);
        SelectNewValue();
    }

    private void CreateValues(string level)
    {
        TextAsset asset = Resources.Load<TextAsset>(level);
        string assetText = asset.ToString();
        string[] assetTextLines = Regex.Split(assetText, Environment.NewLine);
        foreach (string line in assetTextLines)
        {
            string[] split = line.Trim().Split(',');
            Value value = new Value(split[0], split[1]);
            availableValues.Add(value);
        }
    }

    public void SelectNewValue()
    {
        if (currentValue != null)
        {
            usedValues.Add(currentValue);
            availableValues.Remove(currentValue);
        }
        int index = random.Next(availableValues.Count);
        currentValue = availableValues[index];
        goldBarText.text = currentValue.getValue();
        Debug.Log("Current Value: " + currentValue);
    }

}
