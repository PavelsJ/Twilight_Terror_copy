using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mimic_Cutscene_3 : MonoBehaviour
{
    public Enemy_Mimic_Boss bossMovement;
    public Animator animator;

    private void OnEnable()
    {
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.Twilight);
        Music_Manager.instance.PlayMusic(Music_Manager.instance.bossMusic, Music_Manager.MusicState.Boss);
    }

    public void ActivateBoss()
    {
        Music_Manager.instance.PlayOneSound(Music_Manager.SoundType.Warning, 1);
        
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
