using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestCollision : MonoBehaviour
{
    private GameObject bar;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    private void Deposit()
    {

            //animator.ResetTrigger("Close");
            //animator.SetTrigger("Open");

            //animator.ResetTrigger("Open");
            //animator.SetTrigger("Close");

    }

    public void OpenChestAnimation()
    {
        animator.ResetTrigger("Open");
        animator.ResetTrigger("Shake");
        animator.SetTrigger("Open");
    }

    public void CloseChestAnimation()
    {
    }

    public void ShakeChestAnimation()
    {
        animator.ResetTrigger("Shake");
        animator.ResetTrigger("Open");
        animator.SetTrigger("Shake");
    }



}
