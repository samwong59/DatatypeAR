using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterInfo : MonoBehaviour
{
    public string correctAttack;
    private Animator animator;
    [SerializeField]
    public int health;
    public string name;
    [SerializeField]
    private GameObject flameThrowerPrefab;
    [SerializeField]
    private GameObject HUD;
    [SerializeField]
    private TMP_Text healthText;
    [SerializeField]
    private TMP_Text nameText;

    private void OnCollisionEnter(Collision collision)
    {

        if (health <= 0)
        {
            Die();
        } else
        {
            Attack();
            animator.SetBool("isDefending", false);
        }

        StartCoroutine(DestroyProjectile(collision.gameObject));
    }

    private IEnumerator DestroyProjectile(GameObject gameObject)
    {
        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        nameText.text = name;
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

    public void EnterBlockStance()
    {
        animator.SetBool("isDefending", true);
    }

    public void SelectTarget()
    {
        HUD.SetActive(true);
        healthText.text = health.ToString();
    }

    public void DeselectTarget()
    {
        HUD.SetActive(false);
    }
}
