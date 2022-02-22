using System.Collections;
using UnityEngine;
using Firebase.Auth;
using Firebase.Firestore;



public class ChestLevelMenuHandler : MenuHandler
{

    [SerializeField]
    private int passingScore;
    private int score;

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
}
