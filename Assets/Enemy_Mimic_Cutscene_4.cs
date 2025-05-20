using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mimic_Cutscene_4 : MonoBehaviour
{
    public GameObject whiteScreen;
    public ParticleSystem ps;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        ps.Stop();
    }

    public void KillMimic()
    {
        animator.SetTrigger("Death");
        whiteScreen.SetActive(true);
        ps.Play();
    }
}
