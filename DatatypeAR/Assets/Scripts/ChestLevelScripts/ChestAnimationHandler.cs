using UnityEngine;

public class ChestAnimationHandler : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void OpenAndCloseChestAnimation()
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

    public void OpenChest()
    {
        animator.SetBool("isOpen", true);
    }

    public void CloseChest()
    {
        animator.SetBool("isOpen", false);
    }
}
