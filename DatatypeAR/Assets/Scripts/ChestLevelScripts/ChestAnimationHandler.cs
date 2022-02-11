using UnityEngine;

public class ChestAnimationHandler : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void OpenChestAnimation()
    {
        animator.ResetTrigger("Open");
        animator.ResetTrigger("Shake");
        animator.SetTrigger("Open");
    }

    public void ShakeChestAnimation()
    {
        animator.ResetTrigger("Shake");
        animator.ResetTrigger("Open");
        animator.SetTrigger("Shake");
    }
}
