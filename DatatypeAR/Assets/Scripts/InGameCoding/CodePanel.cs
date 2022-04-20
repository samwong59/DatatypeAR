using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodePanel : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;

    public void ToggleHideMenu()
    {
        Animator animator = panel.GetComponent<Animator>();
        bool isOpen = animator.GetBool("isMenuShown");
        animator.SetBool("isMenuShown", !isOpen);
    }
}
