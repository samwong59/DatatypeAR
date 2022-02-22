using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class IfStatementLevel : MonoBehaviour
{
    [SerializeField]
    private GameObject levelPrefab;
    [SerializeField]
    private GameObject tickPrefab;
    [SerializeField]
    private GameObject crossPrefab;

    private GameObject placedLevel;
    private GameObject placedCross;
    private GameObject placedTick;
    private List<GameObject> printStatementValues = new List<GameObject>();

    private ARRaycastManager mRaycastManager;
    private Vector2 touchPosition;
    private RaycastHit hitObject;
    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private ARSessionOrigin mSessionOrigin;

    [SerializeField]
    private GameObject preLevelCanvas;
    [SerializeField]
    private GameObject levelCanvas;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject codeCanvas;
    [SerializeField]
    private GameObject statusPanel;
    [SerializeField]
    private TMP_Text statusText;
    [SerializeField]
    private TMP_Text objectiveText;
    [SerializeField]
    GameObject menuHandlerObject;

    private int levelStage = 0;
    private int correctAnswer;

    private GameObject codeBlocks;

    private void Awake()
    {
        mRaycastManager = GetComponent<ARRaycastManager>();
        mSessionOrigin = GetComponent<ARSessionOrigin>();
        codeBlocks = codeCanvas.transform.GetChild(0).GetChild(1).gameObject;
        
    }
    private void Update()
    {
        if (preLevelCanvas.activeSelf || pauseMenu.activeSelf)
        {
            return;
        }

        Touch touch;
        if (Input.touchCount < 1)
        {
            return;
        }

        touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            if (placedLevel == null)
            {
                if (mRaycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds))
                {
                    CreateAnchor(hits[0]);
                    levelCanvas.SetActive(true);
                    codeCanvas.SetActive(true);
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out hitObject,50.0f))
            {
                if (hitObject.transform.name == "3DText")
                {
                    if (printStatementValues.IndexOf(hitObject.transform.gameObject) == correctAnswer) {
                        StartCoroutine(PrintStatementAnswer(true));
                    }
                    else
                    {
                        StartCoroutine(PrintStatementAnswer(false));
                    }
                }
            }
        }
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        ARAnchor anchor;

        placedLevel = Instantiate(levelPrefab);
        placedLevel.transform.position = new Vector3(placedLevel.transform.position.x, placedLevel.transform.position.y, placedLevel.transform.position.z + 0.225f);

        placedCross = Instantiate(crossPrefab);
        placedCross.transform.position = new Vector3(placedLevel.transform.position.x, placedLevel.transform.position.y + 0.1f, placedLevel.transform.position.z + 0.225f);
        placedCross.SetActive(false);

        placedTick = Instantiate(tickPrefab);
        placedTick.transform.position = new Vector3(placedLevel.transform.position.x, placedLevel.transform.position.y + 0.1f, placedLevel.transform.position.z + 0.225f);
        placedTick.SetActive(false);

        mSessionOrigin.MakeContentAppearAt(placedLevel.transform, hit.pose.position, hit.pose.rotation);

        anchor = placedLevel.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = placedLevel.AddComponent<ARAnchor>();
        }

        for (int i = 0; i < 3; i++)
        {
            printStatementValues.Add(placedLevel.transform.GetChild(i).GetChild(0).gameObject);  
        }

        levelStage = 1;
        SetLevelStage();

        return anchor;
    }

    private void SetLevelStage()
    {
        switch(levelStage)
        {
            case 1:
                codeBlocks.transform.GetChild(0).gameObject.SetActive(true);
                printStatementValues[0].GetComponent<ThreeDimensionalText>().ChangeText("Positive");
                printStatementValues[1].GetComponent<ThreeDimensionalText>().ChangeText("Nothing");
                printStatementValues[2].GetComponent<ThreeDimensionalText>().ChangeText("");
                printStatementValues[2].SetActive(false);
                correctAnswer = 0;
                objectiveText.text = "Read the code and select the value it prints";
                break;
            case 2:
                TogglePrintStatementValues();
                codeBlocks.transform.GetChild(0).gameObject.SetActive(false);
                codeBlocks.transform.GetChild(1).gameObject.SetActive(true);
                correctAnswer = 1;
                objectiveText.text = "What value does it print now?";
                break;
            case 3:
                codeBlocks.transform.GetChild(1).gameObject.SetActive(false);
                codeBlocks.transform.GetChild(2).gameObject.SetActive(true);
                objectiveText.text = "Finish the elif statement so that it checks if x is equal to zero";
                break;
            case 4:
                codeBlocks.transform.GetChild(2).gameObject.SetActive(false);
                codeBlocks.transform.GetChild(3).gameObject.SetActive(true);
                objectiveText.text = "Write an else statement which prints \"Negative\" when ran";
                break;
            case 5:
                TogglePrintStatementValues();
                codeBlocks.transform.GetChild(3).gameObject.SetActive(false);
                codeBlocks.transform.GetChild(4).gameObject.SetActive(true);
                printStatementValues[1].GetComponent<ThreeDimensionalText>().ChangeText("Negative");
                printStatementValues[2].GetComponent<ThreeDimensionalText>().ChangeText("Zero");
                printStatementValues[2].SetActive(true);
                correctAnswer = 1;
                objectiveText.text = "What value does the code print?";
                break;
            case 6:
                TogglePrintStatementValues();
                codeBlocks.transform.GetChild(4).gameObject.SetActive(false);
                codeBlocks.transform.GetChild(5).gameObject.SetActive(true);
                correctAnswer = 2;
                objectiveText.text = "What value does it print now?";
                break;
            case 7:
                TogglePrintStatementValues();
                codeBlocks.transform.GetChild(5).gameObject.SetActive(false);
                codeBlocks.transform.GetChild(6).gameObject.SetActive(true);
                objectiveText.text = "We can write the same program with nested if statements, what value does this print?";
                correctAnswer = 0;
                break;
            case 8:
                TogglePrintStatementValues();
                codeBlocks.transform.GetChild(6).gameObject.SetActive(false);
                codeBlocks.transform.GetChild(7).gameObject.SetActive(true);
                correctAnswer = 1;
                objectiveText.text = "What value does it print now?";
                break;
            case 9:
                menuHandlerObject.GetComponent<IfStatementWalkthroughMenuHandler>().FinishLevel();
                break;
        }
    }

    private void TogglePrintStatementValues()
    {
        foreach (GameObject printStatementValue in printStatementValues)
        {
            printStatementValue.SetActive(!printStatementValue.activeSelf);
        }
    }

    private IEnumerator PrintStatementAnswer(bool isCorrect)
    {
        TogglePrintStatementValues();

        if (isCorrect)
        {
            SoundManagerScript.PlaySound("correct");
            placedTick.SetActive(true);

            yield return new WaitForSeconds(1);

            placedTick.SetActive(false);
            placedCross.SetActive(false);

            levelStage++;
            SetLevelStage();
        }
        else
        {
            SoundManagerScript.PlaySound("incorrect");
            placedCross.SetActive(true);


            yield return new WaitForSeconds(1);

            placedTick.SetActive(false);
            placedCross.SetActive(false);

            objectiveText.text = "Try again";
            TogglePrintStatementValues();
        }
    }

    public void SubmitCode() 
    {
        switch(levelStage)
        {
            case 3:
                string inputText = codeBlocks.transform.GetChild(2).GetChild(1).GetComponent<CodeInputField>().inputField.text.Replace(" ", "").ToLower();
                if (Equals(inputText, "x==0:")) {
                    StartCoroutine(StatusUpdate("Well done", 1.5f));
                    SoundManagerScript.PlaySound("correct");
                    levelStage++;
                    SetLevelStage();
                } else if (Equals(inputText, "x==0"))
                {
                    StartCoroutine(StatusUpdate("Don't forget the colon!", 3));
                    SoundManagerScript.PlaySound("incorrect");
                } else
                {
                    StartCoroutine(StatusUpdate("Try again", 1.5f));
                    SoundManagerScript.PlaySound("incorrect");
                }
                break;
            case 4:
                string inputText1 = codeBlocks.transform.GetChild(3).GetChild(1).GetComponent<CodeInputField>().inputField.text.Replace(" ", "");
                string inputText2 = codeBlocks.transform.GetChild(3).GetChild(2).GetComponent<CodeInputField>().inputField.text.Replace(" ", "");
                if (Equals(inputText1, "else:"))
                {
                    if (Equals(inputText2, "print(\"Negative\")") || Equals(inputText2, "print('Negative')"))
                    {
                        StartCoroutine(StatusUpdate("Well done", 1.5f));
                        SoundManagerScript.PlaySound("correct");
                        levelStage++;
                        SetLevelStage();
                    }
                    else if (Equals(inputText2, "Print(\"Negative\")") || Equals(inputText2, "Print('Negative')"))
                    {
                        StartCoroutine(StatusUpdate("print shouldn't be capitalised", 2.5f));
                        SoundManagerScript.PlaySound("incorrect");
                    }
                    else
                    {
                        StartCoroutine(StatusUpdate("Try again", 1.5f));
                        SoundManagerScript.PlaySound("incorrect");
                    }
                }
                else if (Equals(inputText1, "else"))
                {
                    StartCoroutine(StatusUpdate("Don't forget the colon!", 2.5f));
                    SoundManagerScript.PlaySound("incorrect");
                }
                else
                {
                    StartCoroutine(StatusUpdate("Try again", 1.5f));
                    SoundManagerScript.PlaySound("incorrect");
                }
                break;
        }
    }

    private IEnumerator StatusUpdate(string text, float waitLength)
    {
        statusPanel.SetActive(true);
        statusText.text = text;

        yield return new WaitForSeconds(waitLength);

        statusPanel.SetActive(false);

    }
}
