using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_Switch_Cutscene : MonoBehaviour
{
    public GameObject lightSwitchParts;
    public GameObject lightSwitch;
    
    private int currentPartCount = 0;
    
    private FOD_Agent agent;
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<FOD_Agent>();
        
        lightSwitchParts.SetActive(false);
    }

    public void AddPart(int count)
    {
        currentPartCount += count;
        Debug.Log(currentPartCount);

        if (currentPartCount == 3)
        {
            lightSwitch.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    public void OnCutscene1()
    {
        animator.SetTrigger("Cutscene1");
    }

    public void OnCutscene2()
    {
       agent.EndAgent();
    }
    
    public void OnCutscene3()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        lightSwitchParts.SetActive(true);

        animator.SetTrigger("Cutscene2");
    }
    
    // private IEnumerator CutsceneCoroutine()
    // { 
    //     yield return new WaitForSeconds(0.5f);
    //     GetComponent<FOD_Agent>().EndAgent();
    //     
    //     yield return new WaitForSeconds(1.8f);
    //     GetComponent<SpriteRenderer>().enabled = false;
    //     lightSwitchParts.SetActive(true);
    //     
    //     animator.SetTrigger("Cutscene2");
    // }
}