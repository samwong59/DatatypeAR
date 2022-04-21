using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ListLevel : MonoBehaviour
{
    [SerializeField]
    private GameObject elementPrefab;
    [SerializeField]
    private GameObject elementInChestPrefab;
    [SerializeField]
    private GameObject elementInStackPrefab;
    [SerializeField]
    private GameObject chestPrefab;

    [SerializeField]
    private GameObject preLevelCanvas;
    [SerializeField]
    private GameObject pauseMenu;
    [SerializeField]
    private GameObject levelCanvas;
    [SerializeField]
    private GameObject functionPanel;
    [SerializeField]
    private GameObject appendPanel;
    [SerializeField]
    private TMP_Text objectiveText;
    [SerializeField]
    private TMP_Text functionText;

    private ARSessionOrigin mSessionOrigin;

    private int stage = 1;

    private ARAnchorManager arAnchorManager;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Camera firstPersonCamera;

    private ARRaycastManager raycastManager;

    private List<Element> elements = new List<Element>()
    {
        new Element("Orange", 1),
        new Element("Banana", 1),
        new Element("Carrot", 2),
        new Element("Strawberry", 1),
        new Element("Broccoli", 2),
        new Element("Onion", 2),
        new Element("Apple", 1),
        new Element("Plum", 1),
        new Element("Potato", 2),
        new Element("Leek", 2)
    };
    private GameObject selectedWorldElement;

    private GameObject emptyGameObject;
    private GameObject chest1;
    private GameObject chest2;

    private int correctAnswers = 0;

    private List<GameObject> worldElementsInArea = new List<GameObject>();
    private List<GameObject> worldElementsInChest = new List<GameObject>();
    private List<Element> stackElements = new List<Element>();
    private List<GameObject> worldElementsInStack = new List<GameObject>();

    private int functionQuestionStage = 1;

    [SerializeField]
    private GameObject menuHandler;

    [SerializeField]
    private LineRenderer raycastLine;


    protected class Element
    {
        private string value;
        private int correctList;

        public Element(string value, int correctList)
        {
            this.value = value;
            this.correctList = correctList;
        }

        public string GetValue()
        {
            return value;
        }

        public void SetValue(string value)
        {
            this.value = value;
        }

        public int GetCorrectList()
        {
            return correctList;    
        }

        public void SetCorrectList(int correctList)
        {
            this.correctList = correctList;
        }
    }
    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        arAnchorManager = GetComponent<ARAnchorManager>();
        mSessionOrigin = GetComponent<ARSessionOrigin>();
        firstPersonCamera = GameObject.Find("AR Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        if (preLevelCanvas.activeSelf || pauseMenu.activeSelf)
        {
            return;
        }

        Ray targetRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));

        switch (stage)
        {
            case 1:
                if (raycastManager.Raycast(targetRay, hits, TrackableType.PlaneWithinPolygon)) {
                    foreach (GameObject text in worldElementsInArea)
                    {
                        if (Vector3.Distance(text.transform.position, hits[0].pose.position) < 1.1) //Check there is sufficient euclidian distance between words
                        {
                            return;
                        }
                    }
                    CreateAnchor(hits[0]);
                    if (worldElementsInArea.Count == 10) //Switch stage once 10 words are in the environment
                    {
                        stage = 2;
                        objectiveText.text = "Tap an open space in the room!";
                    }   
                }
                break;
            case 2:
                Touch touch;
                if (Input.touchCount < 1)
                {
                    return;
                }

                touch = Input.GetTouch(0);
                {
                    if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinBounds))
                    {
                        CreateAnchor(hits[0]);
                        stage = 3;
                        objectiveText.text = "Append the elements to the correct list!";
                        appendPanel.SetActive(true);
                    } 
                }
                break;
            case 3:
                RaycastHit hit;
                if (correctAnswers == 10)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Destroy(worldElementsInArea[i]);
                        Destroy(worldElementsInChest[i]);
                    }
                    Destroy(chest1);
                    Destroy(chest2);
                    stage = 4;
                    CreateAnchor(hits[0]);
                    appendPanel.SetActive(false);
                    functionPanel.SetActive(true);
                    objectiveText.text = "Select the correct index returned by the function";
                    break;
                }
                if (Physics.Raycast(targetRay, out hit, 50f))
                {
                    if (hit.transform.name == "3DTextEnvironment(Clone)")
                    {
                        if (selectedWorldElement != null && selectedWorldElement != hit.transform.gameObject)
                        {
                            selectedWorldElement.GetComponent<ThreeDimensionalTextEnvironment>().HideHighlight();
                        }
                        hit.transform.gameObject.GetComponent<ThreeDimensionalTextEnvironment>().ShowHighlight();
                        selectedWorldElement = hit.transform.gameObject;
                        chest1.GetComponentInChildren<ChestAnimationHandler>().CloseChest();
                        chest2.GetComponentInChildren<ChestAnimationHandler>().CloseChest();
                    }
                    else if (hit.transform.name == "Chest")
                    {
                        hit.transform.gameObject.GetComponent<ChestAnimationHandler>().OpenChest();
                    }
                }
                else
                {
                    selectedWorldElement = null;
                    foreach (GameObject e in worldElementsInArea)
                    {
                        e.transform.gameObject.GetComponent<ThreeDimensionalTextEnvironment>().HideHighlight();
                    }
                    chest1.GetComponentInChildren<ChestAnimationHandler>().CloseChest();
                    chest2.GetComponentInChildren<ChestAnimationHandler>().CloseChest();
                }
                break;
            case 4:
                if (Physics.Raycast(targetRay, out hit, 50f))
                {
                    if (hit.transform.name == "3DTextInStack(Clone)")
                    {
                        foreach(GameObject e in worldElementsInStack) 
                        {
                            if (!(e.transform.gameObject == hit.transform.gameObject))
                            {
                                e.transform.gameObject.GetComponent<ThreeDimensionalTextEnvironment>().HideHighlight();
                            }
                        }
                        hit.transform.gameObject.GetComponent<ThreeDimensionalTextEnvironment>().ShowHighlight();
                        selectedWorldElement = hit.transform.gameObject;
                    }
                }
                else
                {
                    selectedWorldElement = null;
                    foreach (GameObject e in worldElementsInStack)
                    {
                        e.transform.gameObject.GetComponent<ThreeDimensionalTextEnvironment>().HideHighlight();
                    }
                }
                break;

        }
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        ARAnchor anchor = new ARAnchor();
        switch(stage)
        {
            case 1:
                GameObject text = Instantiate(elementPrefab);
                Element element = elements[worldElementsInArea.Count];
                text.GetComponent<ThreeDimensionalText>().ChangeText(element.GetValue());
                text.transform.position = hit.pose.position;

                anchor = text.GetComponent<ARAnchor>();
                if (anchor == null)
                {
                    anchor = text.AddComponent<ARAnchor>();
                }
                text.GetComponent<ThreeDimensionalTextEnvironment>().correctList = element.GetCorrectList();

                worldElementsInArea.Add(text);
                return anchor;
            case 2:
                emptyGameObject = Instantiate(new GameObject("EmptyGameObject"));

                chest1 = Instantiate(chestPrefab, new Vector3(emptyGameObject.transform.position.x - 0.225f,
                    emptyGameObject.transform.position.y, emptyGameObject.transform.position.z), emptyGameObject.transform.rotation);
                chest1.GetComponentInChildren<TMP_Text>().text = "Fruit";
                chest2 = Instantiate(chestPrefab, new Vector3(emptyGameObject.transform.position.x + 0.225f,
                    emptyGameObject.transform.position.y, emptyGameObject.transform.position.z), emptyGameObject.transform.rotation);
                chest2.GetComponentInChildren<TMP_Text>().text = "Vegetables";

                foreach (Element e in elements)
                {
                    if (e.GetCorrectList() == 1)
                    {
                        GameObject t = Instantiate(elementInChestPrefab);
                        t.GetComponent<ThreeDimensionalText>().ChangeText(e.GetValue());
                        t.transform.position = new Vector3(chest1.transform.position.x, chest1.transform.position.y + 0.2f,
                            chest1.transform.position.z + (0.05f - (elements.IndexOf(e) * 0.01f)));
                        worldElementsInChest.Add(t);
                        t.SetActive(false);
                    } 
                    else
                    {
                        GameObject t = Instantiate(elementInChestPrefab);
                        t.GetComponent<ThreeDimensionalText>().ChangeText(e.GetValue());
                        t.transform.position = new Vector3(chest2.transform.position.x, chest2.transform.position.y + 0.2f,
                            chest2.transform.position.z + (0.05f - (elements.IndexOf(e) * 0.01f)));
                        worldElementsInChest.Add(t);
                        t.SetActive(false);
                    }
                }

                mSessionOrigin.MakeContentAppearAt(emptyGameObject.transform, hit.pose.position, hit.pose.rotation);

                anchor = emptyGameObject.GetComponent<ARAnchor>();
                if (anchor == null)
                {
                    anchor = emptyGameObject.AddComponent<ARAnchor>();
                }
                return anchor;
            case 4:
                foreach(Element e in stackElements)
                {
                    GameObject t = Instantiate(elementInStackPrefab);
                    t.GetComponent<ThreeDimensionalText>().ChangeText(e.GetValue());
                    t.transform.position = new Vector3(emptyGameObject.transform.position.x, emptyGameObject.transform.position.y + (1f - (0.2f * worldElementsInStack.Count)), emptyGameObject.transform.position.z);
                    worldElementsInStack.Add(t);
                }
                break;

        }
        return anchor;
    }

    public void SubmitWord(int listNo)
    {
        if (selectedWorldElement != null)
        {
            if (listNo == selectedWorldElement.GetComponent<ThreeDimensionalTextEnvironment>().correctList)
            {
                SoundManagerScript.PlaySound("correct");
                worldElementsInChest[worldElementsInArea.IndexOf(selectedWorldElement)].SetActive(true);
                correctAnswers++;
                if (listNo == 1)
                {
                    stackElements.Add(elements[worldElementsInArea.IndexOf(selectedWorldElement)]);
                }
                selectedWorldElement.SetActive(false);
            } 
            else
            {
                SoundManagerScript.PlaySound("incorrect");
            }
        }
        else
        {
            //StartCoroutine(SelectTargetAlert());
        }
    }

    private IEnumerator SelectTargetAlert()
    {
        objectiveText.text = "You must select a target";
        yield return new WaitForSeconds(2);
        if (stage == 3)
        {
        objectiveText.text = "Append the elements to the correct list!";
        }
    }

    public void SubmitFunctionIndex()
    {
        StartCoroutine(FunctionAnswer());
    }

    private IEnumerator FunctionAnswer()
    {
        switch (functionQuestionStage)
        {
            case 1:
                if (selectedWorldElement == worldElementsInStack[1])
                {
                    functionPanel.SetActive(false);
                    objectiveText.text = "Well done!";
                    functionText.text = "Fruits.pop(0)";
                    functionQuestionStage = 2;
                    SoundManagerScript.PlaySound("correct");
                    yield return new WaitForSeconds(2);
                    objectiveText.text = "Select the correct index returned by the function";
                    functionPanel.SetActive(true);
                }
                else
                {
                    SoundManagerScript.PlaySound("incorrect");
                }
                break;
            case 2:
                if (selectedWorldElement == worldElementsInStack[0])
                {
                    worldElementsInStack.RemoveAt(0);
                    Destroy(selectedWorldElement);
                    functionText.text = "Fruits[-1] = 'Kiwi'";
                    functionQuestionStage = 3;
                    SoundManagerScript.PlaySound("correct");
                    functionPanel.SetActive(false);
                    objectiveText.text = "Well done!";
                    yield return new WaitForSeconds(2);
                    objectiveText.text = "Select the correct index returned by the function";
                    functionPanel.SetActive(true);
                }
                else
                {
                    SoundManagerScript.PlaySound("incorrect");
                }
                break;
            case 3:
                if (selectedWorldElement == worldElementsInStack[3])
                {
                    worldElementsInStack[3].GetComponent<ThreeDimensionalTextEnvironment>().ChangeText("Kiwi");
                    functionText.text = "Next Question";
                    menuHandler.GetComponent<ListLevelMenuHandler>().FinishLevel();
                    SoundManagerScript.PlaySound("correct");
                }
                else
                {
                    SoundManagerScript.PlaySound("incorrect");
                }
                break;
        }
    }
}
