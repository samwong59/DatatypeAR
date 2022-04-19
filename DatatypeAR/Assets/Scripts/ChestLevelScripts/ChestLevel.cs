using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using System.Collections;

public class ChestLevel : MonoBehaviour
{

    //Prefabs to use
    [SerializeField]
    private GameObject levelPrefab;
    [SerializeField]
    private GameObject barPrefab;
    [SerializeField]
    private GameObject tickPrefab;
    [SerializeField]
    private GameObject crossPrefab;
    [SerializeField]
    private GameObject returnValuePrefab;
    [SerializeField]
    private GameObject questionPrefab;

    //Placed prefabs
    private GameObject placedLevel;
    private GameObject placedGoldBar;
    private GameObject placedCross;
    private GameObject placedTick;
    private GameObject placedQuestionText;
    private List<GameObject> placedReturnValues = new List<GameObject>();

    //AR management
    private ARRaycastManager mRaycastManager;
    private Vector2 touchPosition;
    private RaycastHit hitObject;
    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private ARSessionOrigin mSessionOrigin;


    private TMP_Text goldBarText;
    private BarValueHandler goldBarScript;

    [SerializeField]
    private GameObject preLevelCanvas;
    [SerializeField]
    private GameObject levelCanvas;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private TMP_Text scoreText;
    [SerializeField]
    GameObject menuHandlerObject;
    [SerializeField]
    GameObject objectivePanel;
    [SerializeField]
    TMP_Text objectiveText;

    public int score = 0;
    [SerializeField]
    private bool isTimerLevel;
    private int questionNumber = 1;


    [SerializeField]
    private string valuesFilePath;
    BarValueHandler.Value currentValue;

    private void Awake()
    {
        mRaycastManager = GetComponent<ARRaycastManager>();
        mSessionOrigin = GetComponent<ARSessionOrigin>();
    }

    private void Update()
    {
        //Check if level has started or is paused
        if (preLevelCanvas.activeSelf || pauseMenu.activeSelf)
        {
            return;
        }

        //Check if level is waiting for next value 
        if (placedGoldBar != null)
        {
            if (!placedGoldBar.activeSelf && isTimerLevel)
            {
                return;
            }
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
                    return;
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out hitObject, 50.0f))
            {
                if (placedGoldBar.activeSelf)
                {
                    if (hitObject.transform.name == "intChest" || hitObject.transform.name == "floatChest" || hitObject.transform.name == "strChest" || hitObject.transform.name == "falseChest" || hitObject.transform.name == "trueChest")
                    {
                        if (Equals((currentValue.GetDataType() + "Chest"), hitObject.transform.name))
                        {
                            hitObject.transform.gameObject.GetComponent<ChestAnimationHandler>().OpenChestAnimation();
                            StartCoroutine(HideGoldBar(true));
                        }
                        else
                        {
                            hitObject.transform.gameObject.GetComponent<ChestAnimationHandler>().ShakeChestAnimation();
                            StartCoroutine(HideGoldBar(false));
                        }
                    }
                }
                if (hitObject.transform.name == "3DText(Clone)")
                {
                    Debug.LogWarning(" Index = " + placedReturnValues.IndexOf(hitObject.transform.gameObject));
                    if (Equals(currentValue.GetReturnValues()[placedReturnValues.IndexOf(hitObject.transform.gameObject)].GetDataType(), "true"))
                    {
                        StartCoroutine(HideReturnValues(true));
                    } else
                    {
                        StartCoroutine(HideReturnValues(false));
                    }
                }
            }
        }
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        ARAnchor anchor;

        GameObject emptyGameObject = Instantiate(new GameObject("Empty"));

        placedLevel = Instantiate(levelPrefab);
        placedLevel.transform.position = new Vector3(emptyGameObject.transform.position.x - 0.225f, emptyGameObject.transform.position.y, emptyGameObject.transform.position.z);

        placedGoldBar = Instantiate(barPrefab);
        placedGoldBar.transform.position = new Vector3(placedLevel.transform.position.x + 0.225f, placedLevel.transform.position.y + 0.2f, placedLevel.transform.position.z + 0.225f);
        goldBarScript = placedGoldBar.GetComponent<BarValueHandler>();
        goldBarScript.InitialValue(valuesFilePath, isTimerLevel);
        currentValue = goldBarScript.currentValue;

        placedCross = Instantiate(crossPrefab);
        placedCross.transform.position = new Vector3(placedLevel.transform.position.x + 0.225f, placedLevel.transform.position.y + 0.2f, placedLevel.transform.position.z + 0.225f);
        placedCross.SetActive(false);

        placedTick = Instantiate(tickPrefab);
        placedTick.transform.position = new Vector3(placedLevel.transform.position.x + 0.225f, placedLevel.transform.position.y + 0.2f, placedLevel.transform.position.z + 0.225f);
        placedTick.SetActive(false);
        if (!isTimerLevel)
        {
            placedQuestionText = Instantiate(questionPrefab);
            placedQuestionText.transform.position = new Vector3(placedLevel.transform.position.x + 0.225f, placedLevel.transform.position.y + 0.25f, placedLevel.transform.position.z + 0.225f);
            placedQuestionText.SetActive(false);
            placedQuestionText.GetComponent<ThreeDimensionalText>().ChangeText("What does " + currentValue.GetValue() + " return?");
            GameObject placedText = Instantiate(returnValuePrefab);
            placedText.transform.position = new Vector3(placedLevel.transform.position.x + 0f, placedLevel.transform.position.y + 0.45f, placedLevel.transform.position.z + 0.225f);
            placedReturnValues.Add(placedText);
            placedText = Instantiate(returnValuePrefab);
            placedText.transform.position = new Vector3(placedLevel.transform.position.x + 0.225f, placedLevel.transform.position.y + 0.6f, placedLevel.transform.position.z + 0.26f);
            placedReturnValues.Add(placedText);
            placedText = Instantiate(returnValuePrefab);
            placedText.transform.position = new Vector3(placedLevel.transform.position.x + 0.5f, placedLevel.transform.position.y + 0.45f, placedLevel.transform.position.z + 0.225f);
            placedReturnValues.Add(placedText);
            for (int i = 0; i <= 2; i++)
            {
                placedReturnValues[i].GetComponent<ThreeDimensionalText>().ChangeText(currentValue.GetReturnValues()[i].GetValue());
                placedReturnValues[i].SetActive(false);
            }
        }

        mSessionOrigin.MakeContentAppearAt(emptyGameObject.transform, hit.pose.position, hit.pose.rotation);

        StartCoroutine(SetObjectiveText("Tap the chest which matches the type of the code on the gold bar"));

        anchor = emptyGameObject.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = emptyGameObject.AddComponent<ARAnchor>();
        }

        return anchor;
    }

    IEnumerator HideGoldBar(bool isCorrect)
    {
        placedGoldBar.SetActive(false);

        if (isCorrect)
        {
            SoundManagerScript.PlaySound("correct");
            placedTick.SetActive(true);
            score++;
        } else
        {
            SoundManagerScript.PlaySound("incorrect");
            placedCross.SetActive(true);
        }

        yield return new WaitForSeconds(1);

        placedTick.SetActive(false);
        placedCross.SetActive(false);
        if (!isTimerLevel)
        {
            questionNumber++;


            if (questionNumber == 2)
            {
                StartCoroutine(SetObjectiveText("What is the value returned by the code?"));
            }

            scoreText.text = score + "/40";
            placedQuestionText.SetActive(true);
            foreach (GameObject returnValue in placedReturnValues)
            {
                returnValue.SetActive(true);
            }
        } else
        {
            scoreText.text = "Score: " + score;
            goldBarScript.SelectNewValue();
            currentValue = goldBarScript.currentValue;
            placedGoldBar.SetActive(true);
        }
    }

    IEnumerator HideReturnValues(bool correctAnswer)
    {

        questionNumber++;
        placedQuestionText.SetActive(false);
        foreach (GameObject returnValue in placedReturnValues)
        {
            returnValue.SetActive(false);
        }

        if (correctAnswer)
        {
            SoundManagerScript.PlaySound("correct");
            placedTick.SetActive(true);
            score++;
            scoreText.text = score + "/40"; 
        }
        else
        {
            SoundManagerScript.PlaySound("incorrect");
            placedCross.SetActive(true);
        }

        yield return new WaitForSeconds(1);

        if (questionNumber == 41)
        {
            menuHandlerObject.GetComponent<ChestLevelMenuHandler>().FinishLevel();
        }

        placedTick.SetActive(false);
        placedCross.SetActive(false);
        goldBarScript.SelectNewValue();
        currentValue = goldBarScript.currentValue;
        placedQuestionText.GetComponent<ThreeDimensionalText>().ChangeText("What does " + currentValue.GetValue() + " return?");
        for (int i = 0; i <= 2; i++)
        {
            placedReturnValues[i].GetComponent<ThreeDimensionalText>().ChangeText(currentValue.GetReturnValues()[i].GetValue());
        }
        placedGoldBar.SetActive(true);
    }

    private IEnumerator SetObjectiveText(string objective)
    {
        objectivePanel.SetActive(true);
        objectiveText.text = objective;

        yield return new WaitForSeconds(3);

        objectivePanel.SetActive(false);
    }

}
