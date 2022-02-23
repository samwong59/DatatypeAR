using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{

    private Camera firstPersonCamera;
    [SerializeField]
    bool yRotation;

    void Start()
    {
        firstPersonCamera = GameObject.Find("AR Camera").GetComponent<Camera>();
    }

    void Update()
    {
        Vector3 direction = firstPersonCamera.transform.position - transform.position;
        if (!yRotation)
        {
            direction.y = 0;
        }
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 2f * Time.deltaTime);
    }
}
