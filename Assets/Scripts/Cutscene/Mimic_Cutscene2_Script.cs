using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic_Cutscene2_Script : Change_Room_Script
{
    
    public Shake_Camera_Manager shakeCamera;
    public Light_Switch_Cutscene lightDestruction;
    public Enemy_Mimic_Cutscene_1 cutsceneMimic;
    
    protected override void ChangeRoom()
    {
        isActive = true;
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        player.GetComponent<FOD_Agent>().SetMinRadiusValue();

        StartCoroutine(DisableDelay(transitionTime + 2.4f));
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.Noise);
        
        cutsceneMimic.gameObject.SetActive(true);
        cutsceneMimic.OnCutscene();
        
        lightDestruction.OnCutscene1();
       
        yield return new WaitForSeconds(0.6f);
        Music_Manager.instance.PlayOneSound(Music_Manager.SoundType.Warning);
        lightDestruction.OnCutscene2();
        
        yield return new WaitForSeconds(0.6f);
        Music_Manager.instance.PlayOneSound(Music_Manager.SoundType.Warning);
        
        yield return new WaitForSeconds(0.8f);
        Music_Manager.instance.PlayOneSound(Music_Manager.SoundType.Warning);
        
        yield return new WaitForSeconds(0.4f);
        lightDestruction.OnCutscene3();
        
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.Twilight);
        
        yield return new WaitForSeconds(transitionTime);
        
        gridInteraction.ChangeSectorState(nextRoomIndex);
        player.GetComponent<FOD_Agent>().SetMaxRadiusValue();
    }
}
