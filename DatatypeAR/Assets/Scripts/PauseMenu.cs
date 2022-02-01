using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    private bool isGamePaused = false;

    [SerializeField]
    private GameObject levelCanvas;

    [SerializeField]
    private GameObject pauseCanvas;

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
