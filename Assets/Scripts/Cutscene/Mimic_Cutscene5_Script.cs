using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic_Cutscene5_Script : Change_Room_Script
{
    public Shake_Camera_Manager shakeCamera;
    public Enemy_Mimic_Cutscene_3 cutscene;
    public Light_Switch_Interaction lightDeactivation;
    
    public GameObject cutsceneCamera;
    public GameObject mainCamera;
    protected override void ChangeRoom()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        player.GetComponent<FOD_Agent>().SetMinRadiusValue();
        
        StartCoroutine(DisableDelay(transitionTime + 1.6f));
        shakeCamera.ShakeCamera(0.5f);
        
        lightDeactivation.DeactivateSwitch();
        cutscene.gameObject.SetActive(true);
        
        cutsceneCamera.SetActive(true);
        mainCamera.SetActive(false);
        
        yield return new WaitForSeconds(transitionTime);
        
        cutscene.DeactivateMimic();
        
        yield return new WaitForSeconds(1f);
        
        cutscene.ActivateBoss();
        
        yield return new WaitForSeconds(0.4f);
        
        shakeCamera.ShakeCamera(0);
        gridInteraction.ChangeSectorState(nextRoomIndex);
        
        yield return new WaitForSeconds(0.2f);
        
        cutsceneCamera.SetActive(false);
        mainCamera.SetActive(true);
        
        player.GetComponent<FOD_Agent>().SetMaxRadiusValue();
    }
}
