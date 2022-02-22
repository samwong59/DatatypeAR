using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CodeInputField : MonoBehaviour 
{

    [SerializeField]
    private int characterLimit;
    public TMP_InputField inputField;
    void Start()
    {
        inputField.characterLimit = characterLimit;
    }

}
