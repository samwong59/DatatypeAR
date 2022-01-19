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
        bar = GameObject.FindGameObjectWithTag("Bar");
    }

    private void Deposit()
    {
        if (Vector3.Distance (gameObject.transform.position, bar.transform.position) < 2)
        {
            animator.ResetTrigger("Close");
            animator.SetTrigger("Open");
        } else
        {
            animator.ResetTrigger("Open");
            animator.SetTrigger("Close");
        }
    }

    private void FixedUpdate()
    {
        Deposit();
    }

}
