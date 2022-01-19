using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    float currentTime;
    float startingTime;

    [SerializeField]
    private TMP_Text countdownText;

    private void Start()
    {
        startingTime = 15f;
        currentTime = startingTime;
    }

    private void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("0");

        if(currentTime <= 0)
        {
            currentTime = 0;
            StartCoroutine(FinishLevel());
        }
    }

    private IEnumerator FinishLevel()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("ResultsScreenDatatype");
    }
}
