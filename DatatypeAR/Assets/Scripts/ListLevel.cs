using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ListLevel : MonoBehaviour
{
    [SerializeField]
    private GameObject wordPrefab;

    [SerializeField]
    private GameObject preLevelCanvas;
    [SerializeField]
    private GameObject pauseMenu;

    private ARSessionOrigin mSessionOrigin;

    private bool stage1 = true;
    

    private void Awake()
    {
        mSessionOrigin = GetComponent<ARSessionOrigin>();

        
    }

    private void Update()
    {
        if (preLevelCanvas.activeSelf || pauseMenu.activeSelf)
        {
            return;
        }

        if (stage1)
        {

        }
    }

}
