using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic_Cutscene3_Script : Change_Room_Script
{
    public Shake_Camera_Manager shakeCamera;
    
    [Header("Compounds")]
    public GameObject switchPart;
    public GameObject mimic;
    
    protected override void ChangeRoom()
    {
        isActive = true;
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        player.GetComponent<FOD_Agent>().SetMinRadiusValue();
        
        StartCoroutine(DisableDelay(transitionTime + 2f));
        
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.Noise);
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.Twilight);
        
        FOD_Agent partAgent = switchPart.GetComponent<FOD_Agent>();
        partAgent.deactivateOnEnd = true;
        partAgent.EndAgent();
        
        shakeCamera.ShakeCamera(0.5f);
        
        yield return new WaitForSeconds(transitionTime);
        
        shakeCamera.ShakeCamera(0f);
        
        mimic.SetActive(true);
        
        yield return new WaitForSeconds(1.2f);
        
        Enemy_Mimic_Cutscene_2 cutscene2 =  mimic.GetComponent<Enemy_Mimic_Cutscene_2>();
        cutscene2.OnCutscene();
        
        yield return new WaitForSeconds(0.8f);
        
        player.GetComponent<FOD_Agent>().SetMaxRadiusValue();
    }
}
