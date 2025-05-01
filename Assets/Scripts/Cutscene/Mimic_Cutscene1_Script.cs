using System.Collections;
using System.Collections.Generic;
using FODMapping;
using UnityEngine;

public class Mimic_Cutscene1_Script : Change_Room_Script
{
    public Shake_Camera_Manager shakeCamera;
    public Transform playerTargetPos;
    private FOD_Manager manager;
    
    [Header("Eyes Settings")]
    public GameObject[] eyes;
    
    private void Start()
    {
        manager = FindObjectOfType<FOD_Manager>(true).GetComponent<FOD_Manager>();
    }
    
    protected override void ChangeRoom()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.Twilight);
        
        StartCoroutine(DisableDelay(transitionTime + 1.2f));
        
        shakeCamera.ShakeCamera(0.5f);
        Player_Movement_Manager.Instance.SetNewStats();
        
        float eyeDelay = transitionTime / eyes.Length;
        foreach (GameObject eye in eyes)
        {
            eye.SetActive(true);
            eye.GetComponent<Animator>().SetTrigger("OpenEye");
            yield return new WaitForSeconds(eyeDelay);
        }
        
        yield return new WaitForSeconds(0.8f);
        
        shakeCamera.ShakeCamera(0);
        manager.RemoveAgentsGradually();
        
        yield return new WaitForSeconds(0.4f);
        
        Player_Movement.Instance.movePoint.position = playerTargetPos.position;
        player.transform.position = playerTargetPos.position;
    }
}
