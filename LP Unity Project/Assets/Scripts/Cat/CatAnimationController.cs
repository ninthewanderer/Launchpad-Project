using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatAnimationController : MonoBehaviour
{

    private Animator animator;
     void OnEnable()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("CatAnimationController: No Animator components found in children.");
            return;
        }

        StartCoroutine(CatCycle());
    }

    IEnumerator CatCycle()
    {
        while (true)
        {
            CatSit();
            CatIdle();
        }
    }

    private void OnDisable()
    {
        
    }

    private void CatIdle()
    {
            animator.SetTrigger("TriggerIdle");
    }

    private void CatSit()
    {
            animator.SetTrigger("TriggerSit");
    }
}
