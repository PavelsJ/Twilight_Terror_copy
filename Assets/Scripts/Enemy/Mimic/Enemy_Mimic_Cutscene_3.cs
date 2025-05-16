using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mimic_Cutscene_3 : MonoBehaviour
{
    public Enemy_Mimic_Boss bossMovement;
    public Animator animator;

    public void ActivateBoss()
    {
        bossMovement.gameObject.SetActive(true);
        bossMovement.OnCutscene();
    }

    public void DeactivateMimic()
    {
        FOD_Agent agent = gameObject.GetComponent<FOD_Agent>();
        animator.SetTrigger("Transition");
        
        if (agent != null)
        {
            agent.deactivateOnEnd = true;
            agent.EndAgent();
        }
    }
}
