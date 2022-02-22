using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;
using System.Collections.Generic;

public class MenuHandler : MonoBehaviour
{
    [SerializeField]
    protected GameObject resultCanvas;
    [SerializeField]
    protected GameObject levelCanvas;
    [SerializeField]
    protected GameObject pauseCanvas;
    [SerializeField]
    protected GameObject preLevelCanvas;
    [SerializeField]
    protected GameObject ARSessionOrigin;
    protected private bool isGamePaused = false;
    [SerializeField]
    protected TMP_Text scoreText;
    [SerializeField]
    protected TMP_Text statusText;
    protected FirebaseAuth fAuth;
    [SerializeField]
    protected string nextLevelName;
    protected FirebaseFirestore fStore;
    protected bool isNextLevelUnlocked;
    protected Dictionary<string, object> levelData;

    protected void GetLevelData()
    {
        fStore.Collection("UserLevelData").Document(fAuth.CurrentUser.UserId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            isNextLevelUnlocked = task.Result.GetValue<bool>(nextLevelName);
            levelData = task.Result.ToDictionary();
        });
    }

    protected void updateUserLevelData()
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
