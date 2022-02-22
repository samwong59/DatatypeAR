using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class IfStatementRPGLevel : MonoBehaviour
{

    [SerializeField]
    private GameObject slimePrefab;
    [SerializeField]
    private GameObject batPrefab;
    [SerializeField]
    private GameObject spiderPrefab;
    private List<GameObject> stage1Monsters = new List<GameObject>();

    private GameObject emptyGameObject;
    private GameObject placedMonster;

    private ARRaycastManager mRaycastManager;
    private Vector2 touchPosition;
    private RaycastHit hitObject;
    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private ARSessionOrigin mSessionOrigin;

    private System.Random random = new System.Random();

    private int stage = 1;

    private void Awake()
    {
        mRaycastManager = GetComponent<ARRaycastManager>();
        mSessionOrigin = GetComponent<ARSessionOrigin>();

        stage1Monsters.Add(slimePrefab);
        stage1Monsters.Add(batPrefab);
        stage1Monsters.Add(spiderPrefab);
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
            if (emptyGameObject == null)
            {
                if (mRaycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds))
                {
                        CreateAnchor(hits[0]);
                }
            } else
            {
                SpawnMonster();
            }
        }
    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        ARAnchor anchor;

        emptyGameObject = Instantiate(new GameObject("EmptyGameObject"));
        mSessionOrigin.MakeContentAppearAt(emptyGameObject.transform, hit.pose.position, hit.pose.rotation);

        anchor = emptyGameObject.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = emptyGameObject.AddComponent<ARAnchor>();
        }

        return anchor;
    }

    private void SpawnMonster()
    {
        if (stage == 1)
        {
            Destroy(placedMonster);
            int index = random.Next(stage1Monsters.Count);
            placedMonster = Instantiate(stage1Monsters[index]);
            placedMonster.transform.position = emptyGameObject.transform.position;
            stage1Monsters.RemoveAt(index);
            if (stage1Monsters.Count == 0)
            {
                stage++;
            }
        }
    }

}
