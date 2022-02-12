using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    float currentTime;
    [SerializeField]
    float startingTime;
    [SerializeField]
    private TMP_Text countdownText;
    [SerializeField]
    GameObject menuHandlerObject;

    private void Start()
    {
        currentTime = startingTime;
    }

    private void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("0");

        if(currentTime <= 0)
        {
            currentTime = 0;
            menuHandlerObject.GetComponent<MenuHandler>().FinishLevel();
        }
    }

}
