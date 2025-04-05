using System.Collections;
using System.Collections.Generic;
using FODMapping;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    public float cutsceneTime;
    public Shake_Camera_Manager shakeCamera;
    public Transform playerTargetPos;
    
    private bool isActive = false;
    private FOD_Manager manager;

    private void Start()
    {
        manager = FindObjectOfType<FOD_Manager>(true).GetComponent<FOD_Manager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isActive)
        {
            Music_Manager.instance.PlaySound(Music_Manager.SoundType.Twilight);
            Music_Manager.instance.PlayMusic(Music_Manager.instance.chaseMusic);
            
            StartCoroutine(Delay());
            isActive = true;
        }
    }

    private IEnumerator Delay()
    {
        if (manager == null)
        {
            Debug.Log("FOD_Manager is empty");
            yield break;
        }
        
        Player_Movement.Instance.isDisable = true;
        shakeCamera.ShakeCamera(0.5f);
        
        // cutscene
        
        yield return new WaitForSeconds(cutsceneTime);
        
        shakeCamera.ShakeCamera(0);
        manager.StartCoroutine(manager.DisableWithDelay(true));
    }
}
