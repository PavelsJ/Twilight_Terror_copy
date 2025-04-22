using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic_Cutscene2_Script : Change_Room_Script
{
    public Shake_Camera_Manager shakeCamera;
    public Light_Switch_Destruction lightDestruction; 
    protected override void ChangeRoom()
    {
        isActive = true;
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        player.GetComponent<FOD_Agent>().SetMinRadiusValue();
        
        lightDestruction.OnCutscene();
        StartCoroutine(DisableDelay());
        
        //shakeCamera.ShakeCamera(0.5f);
        
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.Shake);
        
        yield return new WaitForSeconds(transitionTime);
        
        //shakeCamera.ShakeCamera(0);
        
        gridInteraction.ChangeSectorState(0);
        
        player.GetComponent<FOD_Agent>().SetMaxRadiusValue();
    }
}
