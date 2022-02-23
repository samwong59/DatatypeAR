using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterInfo : MonoBehaviour
{
    public string correctAttack;
    private Animator animator;
    public string ifStatementCode;
    

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        ifStatementCode = ifStatementCode.Replace("<br>", "\n");
    }

    public void Die()
    {
        animator.SetBool("isDead", true);
    }

    public void Attack()
    {
        animator.ResetTrigger("attack");
        animator.SetTrigger("attack");
    }
}
