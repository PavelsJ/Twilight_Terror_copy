using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mimic_Cutscene : MonoBehaviour
{
    public GameObject[] mimicParts;

    internal Animator animator;
    internal FOD_Agent agent;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<FOD_Agent>();

        foreach (var t in mimicParts)
        {
            t.transform.parent = null;
        }
    }
    
    public virtual void OnCutscene()
    {
        StartCoroutine(CutsceneCoroutine());
    }

    private IEnumerator CutsceneCoroutine()
    {
        animator.SetTrigger("DestroySwitch");
        
        yield return new WaitForSeconds(1.8f);
        
        agent.deactivateOnEnd = true;
        agent.EndAgent();
        
        foreach (var t in mimicParts)
        {
            t.SetActive(false);
        }
    }
}
