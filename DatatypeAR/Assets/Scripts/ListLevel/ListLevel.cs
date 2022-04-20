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
    private GameObject objectivePanel;
    [SerializeField]
    private TMP_Text objectiveText;

    private ARSessionOrigin mSessionOrigin;

    private int stage = 1;

    private ARAnchorManager arAnchorManager;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Camera firstPersonCamera;

    private ARRaycastManager raycastManager;

    private List<GameObject> worldElements = new List<GameObject>();
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

    private List<GameObject> worldElementsInChest = new List<GameObject>();

    private List<Element> stackElements = new List<Element>();
    private List<GameObject> worldElementsInStack = new List<GameObject>();

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
                    foreach (GameObject text in worldElements)
                    {
                        if (Vector3.Distance(text.transform.position, hits[0].pose.position) < 1.3) //Check there is sufficient euclidian distance between words
                        {
                            return;
                        }
                    }
                    CreateAnchor(hits[0]);
                    if (worldElements.Count == 10) //Switch stage once 10 words are in the environment
                    {
                        stage = 2;
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
                        levelCanvas.SetActive(true);
                        stage = 3;
                    } 
                }
                break;
            case 3:
                RaycastHit hit;
                if (correctAnswers == 10)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Destroy(worldElements[i]);
                        Destroy(worldElementsInChest[i]);
                    }
                    Destroy(chest1);
                    Destroy(chest2);
                    stage = 4;
                    CreateAnchor(hits[0]);
                    break;
                }
                if (Physics.Raycast(targetRay, out hit, 50f))
                {
                    if (hit.transform.name == "3DTextEnvironment(Clone)")
                    {
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
                    foreach (GameObject text in worldElements)
                    {
                        text.transform.gameObject.GetComponent<ThreeDimensionalTextEnvironment>().HideHighlight();
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
                            if (e.transform.gameObject == hit.transform.gameObject)
                            {
                                hit.transform.gameObject.GetComponent<ThreeDimensionalTextEnvironment>().ShowHighlight();
                            } else
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
                Element element = elements[worldElements.Count];
                text.GetComponent<ThreeDimensionalText>().ChangeText(element.GetValue());
                text.transform.position = hit.pose.position;

                anchor = text.GetComponent<ARAnchor>();
                if (anchor == null)
                {
                    anchor = text.AddComponent<ARAnchor>();
                }
                text.GetComponent<ThreeDimensionalTextEnvironment>().correctList = element.GetCorrectList();

                worldElements.Add(text);
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
                worldElementsInChest[worldElements.IndexOf(selectedWorldElement)].SetActive(true);
                selectedWorldElement.SetActive(false);
                correctAnswers++;
                if (listNo == 1)
                {
                    stackElements.Add(elements[worldElements.IndexOf(selectedWorldElement)]);
                }
            } 
            else
            {
                //TODO notify user they selected wrong list;
            }
        }
        else
        {
            //TODO notify user they must be targeting an element!
        }
    }
}
