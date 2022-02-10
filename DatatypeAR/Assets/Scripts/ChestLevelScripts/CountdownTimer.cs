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
    MenuHandler menuHandler;

    private void Start()
    {
        currentTime = startingTime;
        menuHandler = menuHandlerObject.GetComponent<MenuHandler>();
    }

    private void Update()
    {
        currentTime -= 1 * Time.deltaTime;
        countdownText.text = currentTime.ToString("0");

        if(currentTime <= 0)
        {
            currentTime = 0;
            menuHandler.FinishLevel();
        }
    }

}
