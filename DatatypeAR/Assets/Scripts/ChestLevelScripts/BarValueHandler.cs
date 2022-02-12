using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

public class BarValueHandler : MonoBehaviour
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
        Value[] returnValues;

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

        public Value[] getReturnValues()
        {
            return returnValues;
        }

        public void setReturnValues(Value[] returnValues)
        {
            this.returnValues = returnValues;
        }

    }


    public void InitialValue(string level, bool isTimerLevel)
    {
        if (isTimerLevel)
        {
            CreateValuesTimerLevel(level);
        }
        else
        {
            CreateValuesScoreLevel(level);
        }
        SelectNewValue();
    }

    private void CreateValuesTimerLevel(string level)
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

    private void CreateValuesScoreLevel(string level)
    {
        TextAsset asset = Resources.Load<TextAsset>(level);
        string assetText = asset.ToString();
        string[] assetTextLines = Regex.Split(assetText, Environment.NewLine);
        foreach (string line in assetTextLines)
        {
            string[] split = line.Trim().Split(',');
            Value value = new Value(split[0], split[1]);
            Value[] returnValues = new Value[3];
            Value returnValue1 = new Value(split[2], split[3]);
            returnValues[0] = returnValue1;
            Value returnValue2 = new Value(split[4], split[5]);
            returnValues[1] = returnValue2;
            Value returnValue3 = new Value(split[6], split[7]);
            returnValues[2] = returnValue3;
            value.setReturnValues(returnValues);
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
