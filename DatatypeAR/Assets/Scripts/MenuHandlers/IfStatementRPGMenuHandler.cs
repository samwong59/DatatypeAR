using System.Collections;
using UnityEngine;
using Firebase.Auth;
using Firebase.Firestore;

public class IfStatementRPGMenuHandler : MenuHandler
{

    [SerializeField]
    GameObject codeCanvas;

    void Start()
    {
        resultCanvas.SetActive(false);
        levelCanvas.SetActive(false);
        pauseCanvas.SetActive(false);
        codeCanvas.SetActive(false);
        preLevelCanvas.SetActive(true);

        fAuth = FirebaseAuth.DefaultInstance;
        fStore = FirebaseFirestore.DefaultInstance;
        Time.timeScale = 1f;

        GetLevelData();
    }

    public void FinishLevel(bool isGameOver)
    {
        StartCoroutine(FinishLevelLogic(isGameOver));
    }

    private IEnumerator FinishLevelLogic(bool isGameOver)
    {
        levelCanvas.SetActive(false);
        codeCanvas.SetActive(false);
        ARSessionOrigin.GetComponent<IfStatementRPGLevel>().enabled = false;

        yield return new WaitForSeconds(1);

        Time.timeScale = 0f;
        resultCanvas.SetActive(true);
        SetStatusText(isGameOver);
    }

    void SetStatusText(bool isGameOver)
    {
        if (isGameOver)
        {
            statusText.text = "Game over, try again";
        } else
        {
            if (!isNextLevelUnlocked)
            {
                isNextLevelUnlocked = true;
                statusText.text = "Congrats, you unlocked the next level";
                updateUserLevelData();
            }
        }
    }
}
