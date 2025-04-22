using System.Collections;
using System.Collections.Generic;
using FODMapping;
using UnityEngine;

public class Mimic_Cutscene1_Script : Change_Room_Script
{
    public Shake_Camera_Manager shakeCamera;
    public Transform playerTargetPos;
    private FOD_Manager manager;
    
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
        if (manager == null)
        {
            Debug.Log("FOD_Manager is empty");
            yield break;
        }
        
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.Twilight);
        
        StartCoroutine(DisableDelay());
        
        shakeCamera.ShakeCamera(0.5f);
        Player_Movement_Manager.Instance.SetNewStats();
        
        yield return new WaitForSeconds(transitionTime);
        
        shakeCamera.ShakeCamera(0);
        manager.RemoveAgentsGradually();
        
        yield return new WaitForSeconds(0.4f);
        
        Player_Movement.Instance.movePoint.position = playerTargetPos.position;
        player.transform.position = playerTargetPos.position;
        
        Player_Movement.Instance.isDisable = false;
    }
}
