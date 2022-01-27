using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    private bool isGamePaused = false;

    [SerializeField]
    private GameObject levelCanvas;

    [SerializeField]
    private GameObject pauseCanvas;

    private void Start()
    {
        levelCanvas = GameObject.FindGameObjectWithTag("LevelCanvas");
    }

    public void Pause()
    {
        isGamePaused = true;
        Debug.Log("GamePaused: " + isGamePaused);
        GamePause();
    }

    public void Resume()
    {
        isGamePaused = false;
        GamePause();
    }

    public void GamePause()
    {
        if (isGamePaused)
        {
            levelCanvas.SetActive(false);
            pauseCanvas.SetActive(true);
            Time.timeScale = 0f;
        } else
        {
            Time.timeScale = 1f;
            pauseCanvas.SetActive(false);
            levelCanvas.SetActive(true);
        }
    }

    void OnApplicationPause()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }
}
