using System.Collections;
using UnityEngine;
using Firebase.Auth;
using Firebase.Firestore;

public class ListLevelMenuHandler : MenuHandler
{

    [SerializeField]
    private GameObject appendPanel;

    void Start()
    {
        resultCanvas.SetActive(false);
        appendPanel.SetActive(false);
        levelCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        preLevelCanvas.SetActive(true);

        fAuth = FirebaseAuth.DefaultInstance;
        fStore = FirebaseFirestore.DefaultInstance;
        Time.timeScale = 1f;

        GetLevelData();
    }

    public void FinishLevel()
    {
        StartCoroutine(FinishLevelLogic());
    }

    private IEnumerator FinishLevelLogic()
    {
        levelCanvas.SetActive(false);
        ARSessionOrigin.GetComponent<ListLevel>().enabled = false;

        yield return new WaitForSeconds(1);

        Time.timeScale = 0f;
        resultCanvas.SetActive(true);
        SetStatusText();
    }

    void SetStatusText()
    {
        if (!isNextLevelUnlocked)
        {
            isNextLevelUnlocked = true;
            statusText.text = "Congrats, you unlocked the next level";
            updateUserLevelData();
        }
    }
}
