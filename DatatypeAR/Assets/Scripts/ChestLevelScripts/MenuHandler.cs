using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Threading.Tasks;
using System.Collections.Generic;

public class MenuHandler : MonoBehaviour
{

    [SerializeField]
    GameObject resultCanvas;
    [SerializeField]
    GameObject levelCanvas;
    [SerializeField]
    GameObject pauseCanvas;
    [SerializeField]
    GameObject preLevelCanvas;
    [SerializeField]
    GameObject ARSessionOrigin;
    private bool isGamePaused = false;
    [SerializeField]
    TMP_Text scoreText;
    [SerializeField]
    TMP_Text statusText;
    FirebaseAuth fAuth;
    [SerializeField]
    private string nextLevelName;
    private FirebaseFirestore fStore;
    private bool isNextLevelUnlocked;
    private int score;
    private Dictionary<string, object> levelData;
    [SerializeField]
    private int passingScore;

    void Start()
    {
        resultCanvas.SetActive(false);
        levelCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        preLevelCanvas.SetActive(true);

        fAuth = FirebaseAuth.DefaultInstance;
        fStore = FirebaseFirestore.DefaultInstance;
        Time.timeScale = 1f;

        GetLevelData();
    }

    void GetLevelData()
    {
        fStore.Collection("UserLevelData").Document(fAuth.CurrentUser.UserId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            isNextLevelUnlocked = task.Result.GetValue<bool>(nextLevelName);
            levelData = task.Result.ToDictionary();
        });
    }

    public void FinishLevel()
    {
        StartCoroutine(FinishLevelLogic());
    }
    
    private IEnumerator FinishLevelLogic()
    {
        levelCanvas.SetActive(false);
        score = ARSessionOrigin.GetComponent<ChestLevel>().score;
        scoreText.text = "You scored " + score.ToString() + " points";
        ARSessionOrigin.GetComponent<ChestLevel>().enabled = false;

        yield return new WaitForSeconds(1);
        
        Time.timeScale = 0f;
        resultCanvas.SetActive(true);
        SetStatusText();
    }

    void SetStatusText()
    {
        if (!isNextLevelUnlocked)
        {
            if (score >= passingScore)
            {
                isNextLevelUnlocked = true;
                statusText.text = "Congrats, you unlocked the next level";
                updateUserLevelData();
            } else
            {
                statusText.text = "You need to score at least" + passingScore.ToString() +  "to move on to the next level, try again";
            }
        }
    }

    void updateUserLevelData()
    {
        levelData[nextLevelName] = true;
        fStore.Collection("UserLevelData").Document(fAuth.CurrentUser.UserId).UpdateAsync(levelData);
    }

    public void PlayNextLevel()
    {
        if (isNextLevelUnlocked)
        {
            SceneManager.LoadScene(nextLevelName);
        }
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
