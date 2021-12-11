using UnityEngine;
using TMPro;

public class AuthUIManager : MonoBehaviour
{

    public static AuthUIManager instance;

    [Header("References")]
    //[SerializeField]
    //private GameObject checkingForAccountUI;
    [SerializeField]
    private GameObject loginUI;
    [SerializeField]
    private GameObject registerUI;
    [SerializeField]
    private GameObject accountsMenuUI;
    //[SerializeField]
    //private GameObject verifyEmailUI;
    //[SerializeField]
    //private GameObject verifyEmailText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void ClearUI()
    {
        loginUI.SetActive(false);
        registerUI.SetActive(false);
        accountsMenuUI.SetActive(false);
        FirebaseManager.instance.ClearOutputs();
    }

    public void LoginScreen()
    {
        ClearUI();
        loginUI.SetActive(true);
    }

    public void RegisterScreen()
    {
        ClearUI();
        registerUI.SetActive(true);
    }

}
