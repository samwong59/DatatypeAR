using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class ChestLevel : MonoBehaviour
{
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

    private ARRaycastManager raycastManager;
    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private GameObject placedLevel;
    private GameObject placedGoldBar;
    private GameObject placedCross;
    private GameObject placedTick;
    private GameObject placedQuestionText;
    private List<GameObject> placedReturnValues = new List<GameObject>();

    private ARSessionOrigin mSessionOrigin;
    private Vector2 touchPosition;
    private RaycastHit hitObject;
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
    public int score = 0;
    [SerializeField]
    private bool isTimerLevel;
    private int questionNumber = 1;


    [SerializeField]
    private string valuesFilePath;
    BarValueHandler.Value currentValue;

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        mSessionOrigin = GetComponent<ARSessionOrigin>();
    }

    private void Update()
    {
        if (preLevelCanvas.activeSelf || pauseMenu.activeSelf)
        {
            return;
        }

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
                if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds))
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
                        if (Equals((currentValue.getDataType() + "Chest"), hitObject.transform.name))
                        {
                            hitObject.transform.gameObject.GetComponent<ChestAnimationHandler>().OpenChestAnimation();
                            CorrectChestAnswer();
                        }
                        else
                        {
                            hitObject.transform.gameObject.GetComponent<ChestAnimationHandler>().ShakeChestAnimation();
                            IncorrectChestAnswer();
                        }
                }
                }
                if (hitObject.transform.name == "3DText(Clone)")
                {
                    Debug.LogWarning(" Index = " + placedReturnValues.IndexOf(hitObject.transform.gameObject));
                    if (Equals(currentValue.getReturnValues()[placedReturnValues.IndexOf(hitObject.transform.gameObject)].getDataType(), "true"))
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

    private void CorrectChestAnswer()
    {
            StartCoroutine(HideGoldBar(true));
    }

    private void IncorrectChestAnswer()
    {
            StartCoroutine(HideGoldBar(false));
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
            placedQuestionText.GetComponent<ThreeDimensionalText>().ChangeText("What does " + currentValue.getValue() + " return?");
            GameObject placedText = Instantiate(returnValuePrefab);
            placedText.transform.position = new Vector3(placedLevel.transform.position.x + 0f, placedLevel.transform.position.y + 0.45f, placedLevel.transform.position.z + 0.225f);
            placedText.transform.rotation = Quaternion.Euler(0,-15,0);
            placedReturnValues.Add(placedText);
            placedText = Instantiate(returnValuePrefab);
            placedText.transform.position = new Vector3(placedLevel.transform.position.x + 0.225f, placedLevel.transform.position.y + 0.6f, placedLevel.transform.position.z + 0.26f);
            placedReturnValues.Add(placedText);
            placedText = Instantiate(returnValuePrefab);
            placedText.transform.position = new Vector3(placedLevel.transform.position.x + 0.5f, placedLevel.transform.position.y + 0.45f, placedLevel.transform.position.z + 0.225f);
            placedText.transform.rotation = Quaternion.Euler(0, 15, 0);
            placedReturnValues.Add(placedText);
            for (int i = 0; i <= 2; i++)
            {
                placedReturnValues[i].GetComponent<ThreeDimensionalText>().ChangeText(currentValue.getReturnValues()[i].getValue());
                placedReturnValues[i].SetActive(false);
            }
        }

        mSessionOrigin.MakeContentAppearAt(emptyGameObject.transform, hit.pose.position, hit.pose.rotation);


        anchor = emptyGameObject.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = emptyGameObject.AddComponent<ARAnchor>();
        }

        return anchor;
    }

    IEnumerator HideGoldBar(bool correctAnswer)
    {
        placedGoldBar.SetActive(false);

        if (correctAnswer)
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
            menuHandlerObject.GetComponent<MenuHandler>().FinishLevel();
        }

        placedTick.SetActive(false);
        placedCross.SetActive(false);
        goldBarScript.SelectNewValue();
        currentValue = goldBarScript.currentValue;
        placedQuestionText.GetComponent<ThreeDimensionalText>().ChangeText("What does " + currentValue.getValue() + " return?");
        for (int i = 0; i <= 2; i++)
        {
            placedReturnValues[i].GetComponent<ThreeDimensionalText>().ChangeText(currentValue.getReturnValues()[i].getValue());
        }
        placedGoldBar.SetActive(true);
    }
}
