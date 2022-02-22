using System.Collections;
using UnityEngine;
using Firebase.Auth;
using Firebase.Firestore;

public class IfStatementWalkthroughMenuHandler : MenuHandler
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

    public void FinishLevel()
    {
        StartCoroutine(FinishLevelLogic());
    }

    private IEnumerator FinishLevelLogic()
    {
        levelCanvas.SetActive(false);
        codeCanvas.SetActive(false);
        ARSessionOrigin.GetComponent<IfStatementLevel>().enabled = false;

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
