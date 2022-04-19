using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ListLevel : MonoBehaviour
{
    [SerializeField]
    private GameObject wordPrefab;
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

    private List<GameObject> texts = new List<GameObject>();
    private List<Element> elements = new List<Element>()
    {
        new Element("List1", 1),
        new Element("List2", 2),
        new Element("List1", 1),
        new Element("List2", 2),
        new Element("List1", 1),
        new Element("List2", 2),
        new Element("List1", 1),
        new Element("List2", 2),
        new Element("List1", 1),
        new Element("List2", 2)
    };
    private GameObject selectedWord;

    private GameObject chest1;
    private GameObject chest2;

    private int correctAnswers = 0;

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
                    foreach (GameObject text in texts)
                    {
                        if (Vector3.Distance(text.transform.position, hits[0].pose.position) < 1.2) //Check there is sufficient euclidian distance between words
                        {
                            return;
                        }
                    }
                    CreateAnchor(hits[0]);
                    if (texts.Count == 10) //Switch stage once 10 words are in the environment
                    {
                        stage = 2;
                        //foreach (GameObject text in texts)
                        //{
                        //    text.SetActive(true);
                        //}
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
                if (Physics.Raycast(targetRay, out hit, 20f))
                {
                    if (hit.transform.name == ("3DTextEnvironment(Clone)"))
                    {
                        hit.transform.gameObject.GetComponent<ThreeDimensionalTextEnvironment>().ShowHighlight();
                        selectedWord = hit.transform.gameObject;
                    }
                }
                else
                {
                    selectedWord = null;
                    foreach (GameObject text in texts)
                    {
                        text.transform.gameObject.GetComponent<ThreeDimensionalTextEnvironment>().HideHighlight();
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
                GameObject text = Instantiate(wordPrefab);
                Element element = elements[texts.Count];
                text.GetComponent<ThreeDimensionalText>().ChangeText(element.GetValue());
                text.transform.position = hit.pose.position;

                anchor = text.GetComponent<ARAnchor>();
                if (anchor == null)
                {
                    anchor = text.AddComponent<ARAnchor>();
                }
                text.GetComponent<ThreeDimensionalTextEnvironment>().correctList = element.GetCorrectList();

                texts.Add(text);
                return anchor;
            case 2:
                GameObject emptyGameObject = Instantiate(new GameObject("EmptyGameObject"));

                chest1 = Instantiate(chestPrefab, new Vector3(emptyGameObject.transform.position.x - 0.2f,
                    emptyGameObject.transform.position.y, emptyGameObject.transform.position.z), emptyGameObject.transform.rotation);
                chest2 = Instantiate(chestPrefab, new Vector3(emptyGameObject.transform.position.x + 0.2f,
                    emptyGameObject.transform.position.y, emptyGameObject.transform.position.z), emptyGameObject.transform.rotation);

                mSessionOrigin.MakeContentAppearAt(emptyGameObject.transform, hit.pose.position, hit.pose.rotation);

                anchor = emptyGameObject.GetComponent<ARAnchor>();
                if (anchor == null)
                {
                    anchor = emptyGameObject.AddComponent<ARAnchor>();
                }
                return anchor;
        }
        return anchor;
    }

    public void SubmitWord(int listNo)
    {
        if (selectedWord != null)
        {
            if (listNo == selectedWord.GetComponent<ThreeDimensionalTextEnvironment>().correctList)
            {
                Destroy(selectedWord);
                correctAnswers++;
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
