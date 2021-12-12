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
    [SerializeField]
    private GameObject verifyEmailUI;
    [SerializeField]
    private TMP_Text verifyEmailText;

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
        verifyEmailUI.SetActive(false);
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

    public void AwaitVerification(bool isEmailSent, string email, string outputError)
    {
        ClearUI();
        verifyEmailUI.SetActive(true);
        if(isEmailSent)
        {
            verifyEmailText.text = $"Sent Email\nPlease Verifiry {email}";
        }
        else
        {
            verifyEmailText.text = $"Email Not Sent: {outputError}\n Please Verify {email}";
        }
    }

}
