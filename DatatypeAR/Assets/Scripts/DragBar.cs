using UnityEngine;
using TMPro;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;

public class DragBar : MonoBehaviour
{

    private Vector3 mOffset;
    private float mZCoord;
    private GameObject[] chests;
    [SerializeField]
    TMP_Text goldBarText;
    private List<Value> availableValues = new List<Value>();
    private List<Value> usedValues = new List<Value>();
    private System.Random random = new System.Random();
    private Value currentValue;
    [SerializeField]
    private TMP_Text ScoreText;
    private int score = 0;

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


    private void Start()
    {
        chests = GameObject.FindGameObjectsWithTag("Chest");
        CreateValues();
        SelectNewValue();
    }

    private void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        // Store offset - gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - getMouseWorldPos();
    }

    private Vector3 getMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = mZCoord;

        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    private void OnMouseDrag()
    {
        transform.position = getMouseWorldPos() + mOffset;
    }

    private void OnMouseUp()
    {
        foreach (GameObject chest in chests)
        {
            if (Vector3.Distance(gameObject.transform.position, chest.transform.position) < 2) {
            
            gameObject.transform.position = new Vector3(0, 3.3f, (float)-5.5);
            if (currentValue.getDataType() + "Chest" == chest.name)
                {
                    score++;
                    ScoreText.text = "Score: " + score.ToString();
                }
            else
                {
                    Debug.Log("Incorrect");
                }

            SelectNewValue();
            }
        }
    }

    private void CreateValues()
    {
        string path = "Assets/DataTypeObjects/Level1.txt";
        StreamReader reader = new StreamReader(path);
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] split = line.Split(',');
            Value value = new Value(split[0], split[1]);
            availableValues.Add(value);
        }
        reader.Close();
    }

    private void SelectNewValue()
    {
        if (currentValue != null)
        {
            usedValues.Add(currentValue);
            availableValues.Remove(currentValue);
        }
        int index = random.Next(availableValues.Count);
        currentValue = availableValues[index];
        goldBarText.text = currentValue.getValue();
    }

}
