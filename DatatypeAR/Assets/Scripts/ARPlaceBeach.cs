using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using System.Collections;

public class ARPlaceBeach : MonoBehaviour
{
    [SerializeField]
    private GameObject levelPrefab;

    [SerializeField]
    private GameObject barPrefab;

    private ARRaycastManager raycastManager;

    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private GameObject placedLevel;

    private GameObject placedGoldBar;

    private ARSessionOrigin mSessionOrigin;

    private bool onTouchHold = false;

    private Vector2 touchPosition;

    private RaycastHit hitObject;

    private TMP_Text goldBarText;

    private DragBar goldBarScript;

    [SerializeField]
    private GameObject preLevelCanvas;

    [SerializeField]
    private GameObject levelCanvas;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private TMP_Text scoreText;
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
            if (!placedGoldBar.activeSelf)
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
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out hitObject, 50.0f))
            {
                if (hitObject.transform.name == "intChest" || hitObject.transform.name == "floatChest" || hitObject.transform.name == "strChest")
                {
                    if (Equals((goldBarScript.currentValue.getDataType() + "Chest"), hitObject.transform.name))
                    {
                        hitObject.transform.gameObject.GetComponent<ChestCollision>().OpenChestAnimation();
                        StartCoroutine(HideGoldBar());
                        score++;
                        scoreText.text = "Score: " + score;
                        goldBarScript.SelectNewValue();
                    }
                    else
                    {
                        hitObject.transform.gameObject.GetComponent<ChestCollision>().ShakeChestAnimation();
                        StartCoroutine(HideGoldBar());
                        goldBarScript.SelectNewValue();
                    }
                }
            }
        }

        if (touch.phase == TouchPhase.Ended)
        {
            onTouchHold = false;
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

        mSessionOrigin.MakeContentAppearAt(emptyGameObject.transform, hit.pose.position, hit.pose.rotation);


        anchor = emptyGameObject.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = emptyGameObject.AddComponent<ARAnchor>();
        }
        goldBarScript = placedGoldBar.GetComponent<DragBar>();
        goldBarScript.InitialValue();

        return anchor;
    }

    IEnumerator HideGoldBar()
    {
        placedGoldBar.SetActive(false);

        yield return new WaitForSeconds(1);

        placedGoldBar.SetActive(true);
    }
}
