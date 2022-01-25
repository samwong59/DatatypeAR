using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARPlaceBeach : MonoBehaviour
{
    [SerializeField]
    private GameObject levelPrefab;

    private ARRaycastManager raycastManager;

    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private bool isLevelSet = false;

    private ARSessionOrigin mSessionOrigin;

    private void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        mSessionOrigin = GetComponent<ARSessionOrigin>();
    }

    private void Update()
    {
        Touch touch;
        if (Input.touchCount < 1 || (touch = Input.GetTouch(0)).phase != TouchPhase.Began)
        {
            return;
        }

        if (raycastManager.Raycast(touch.position, hits, UnityEngine.XR.ARSubsystems.TrackableType.All))
        {
            if (!isLevelSet)
            {
                CreateAnchor(hits[0]);
            }

            Debug.Log("$Instantiated on: {hits[0].hitType}");
        }

    }

    ARAnchor CreateAnchor(in ARRaycastHit hit)
    {
        ARAnchor anchor;

        //if (hit.trackable is ARPlane plane)
        //{
        //    var planeManager = GetComponent<ARPlaneManager>();
        //    if (planeManager)
        //    {
        //        var anchorManager = GetComponent<ARAnchorManager>();
        //        var oldPrefab = anchorManager.anchorPrefab;
        //        anchorManager.anchorPrefab = levelPrefab;
        //        anchor = anchorManager.AttachAnchor(plane, hit.pose);
        //        anchorManager.anchorPrefab = oldPrefab;

        //        Debug.Log($"Create anchor attachment for plane (id: {anchor.nativePtr}).");
        //        isLevelSet = true;
        //        return anchor;
        //    }
        //}

        var instantiatedObject = Instantiate(levelPrefab);
        instantiatedObject.transform.position = new Vector3(instantiatedObject.transform.position.x - 0.225f, instantiatedObject.transform.position.y, instantiatedObject.transform.position.z - 0.225f);
        mSessionOrigin.MakeContentAppearAt(instantiatedObject.transform, hit.pose.position, hit.pose.rotation);


        anchor = instantiatedObject.GetComponent<ARAnchor>();
        if (anchor == null)
        {
            anchor = instantiatedObject.AddComponent<ARAnchor>();
        }

        Debug.Log($"Created regular anchor (id: {anchor.nativePtr}.");

        isLevelSet = true;

        return anchor;
    }
}
