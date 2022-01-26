using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

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
                }
            }

            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            if (Physics.Raycast(ray, out hitObject, 50.0f))
            {
                if (hitObject.transform.name == "intChest" || hitObject.transform.name == "floatChest" || hitObject.transform.name == "strChest")
                {
                    if (Equals((goldBarScript.currentValue.getDataType() + "Chest"), hitObject.transform.name))
                    {
                        score++;
                        scoreText.text = "Score: " + score;
                        goldBarScript.SelectNewValue();
                    }
                    else
                    {
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

        placedLevel = Instantiate(levelPrefab);
        placedLevel.transform.position = new Vector3(placedLevel.transform.position.x - 0.225f, placedLevel.transform.position.y, placedLevel.transform.position.z - 0.225f);

        placedGoldBar = Instantiate(barPrefab);
        placedGoldBar.transform.position = new Vector3(placedLevel.transform.position.x + 0.225f, placedLevel.transform.position.y + 0.2f, placedLevel.transform.position.z + 0.225f);

        mSessionOrigin.MakeContentAppearAt(placedLevel.transform, hit.pose.position, hit.pose.rotation);


        anchor = placedLevel.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = placedLevel.AddComponent<ARAnchor>();
        }

        Debug.Log("Calling Initial Value Method");
        goldBarScript = placedGoldBar.GetComponent<DragBar>();
        goldBarScript.InitialValue();

        Debug.Log($"Created regular anchor (id: {anchor.nativePtr}.");

        return anchor;
    }
}
