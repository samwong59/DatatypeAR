using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase.Auth;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

public class Menu : MonoBehaviour
{

    private FirebaseAuth fAuth;
    private FirebaseFirestore fStore;
    private Dictionary<string, object> levelData;


    private void Start()
    {
        fAuth = FirebaseAuth.DefaultInstance;
        fStore = FirebaseFirestore.DefaultInstance;

        GetLevelData();
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    void GetLevelData()
    {
        fStore.Collection("UserLevelData").Document(fAuth.CurrentUser.UserId).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            levelData = task.Result.ToDictionary();
        });
    }

    public void PlayLevel(string levelName)
    {
        if ((bool)levelData[levelName])
        {
        SceneManager.LoadScene(levelName);
        }
    }

}
