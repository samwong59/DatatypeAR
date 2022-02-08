using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;

public class MenuHandler : MonoBehaviour
{

    [SerializeField]
    GameObject resultCanvas;
    [SerializeField]
    GameObject levelCanvas;
    [SerializeField]
    GameObject pauseCanvas;
    [SerializeField]
    GameObject ARSessionOrigin;
    private bool isGamePaused = false;
    [SerializeField]
    TMP_Text scoreText;
    [SerializeField]
    TMP_Text statusText;
    FirebaseAuth auth;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        Time.timeScale = 1f;
        statusText.text = auth.CurrentUser.DisplayName;
    }

    public void FinishLevel()
    {
        StartCoroutine(FinishLevelLogic());
    }
    
    private IEnumerator FinishLevelLogic()
    {
        levelCanvas.SetActive(false);
        scoreText.text = "You scored " + ARSessionOrigin.GetComponent<ARPlaceBeach>().score.ToString() + " points";
        ARSessionOrigin.GetComponent<ARPlaceBeach>().enabled = false;

        yield return new WaitForSeconds(3);
        
        Time.timeScale = 0f;
        resultCanvas.SetActive(true);
    }

    public void Pause()
    {
        isGamePaused = true;
        GamePause();
    }

    public void Resume()
    {
        isGamePaused = false;
        GamePause();
    }

    private void GamePause()
    {
        if (isGamePaused)
        {
            levelCanvas.SetActive(false);
            pauseCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
            levelCanvas.SetActive(true);
            pauseCanvas.SetActive(false);
        }
    }

    void OnApplicationPause()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void QuitLevel()
    {
        SceneManager.LoadScene("Menu");
    }
}
