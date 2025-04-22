using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_Switch_Destruction : MonoBehaviour
{
    public GameObject lightSwitchParts;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnCutscene()
    {
        GetComponent<FOD_Agent>().EndAgent();
        GetComponent<SpriteRenderer>().enabled = false;
        
        lightSwitchParts.SetActive(true);
        
        animator.SetTrigger("Cutscene");
    }
}