using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mimic_Cutscene_1 : Enemy_Mimic_Cutscene
{
    public override void OnCutscene()
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
