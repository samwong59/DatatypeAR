using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void PlayLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

}
