using System;
using System.Collections;
using System.Collections.Generic;
using FODMapping;
using UnityEngine;

public class Light_Switch_Interaction : MonoBehaviour
{
    public bool isActive = false;
    public Sprite sprite;
    
    public Bed_Interaction bedInteraction;

    public GameObject dreamSpace;
    public GameObject checkpoint;
    public GameObject hintToShow;
    public GameObject hintToHide;
    
    private FOD_Manager manager;
    private Transform player;

    private void Start()
    {
        manager = FindObjectOfType<FOD_Manager>(true).GetComponent<FOD_Manager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive && other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject.transform;
            
            if (player != null)
            {
                isActive = true;
                OnSceneEnd();
            }
        }
    }

    private void OnSceneEnd()
    {
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.LightSwitch);
        Music_Manager.instance.SetToAmbientMusic();
        
        if (manager == null)
        {
            Debug.Log("FOD_Manager is empty");
            return;
        }

        StartCoroutine(DisableWithDelay());
        
        if (bedInteraction != null)
        {
            bedInteraction.endScene = true;
            Player_Movement_Manager.Instance.isInvulnerable = true;
        }
        
        GetComponent<SpriteRenderer>().sprite = sprite;
        manager.StartCoroutine(manager.DisableWithDelay());
    }

    private IEnumerator DisableWithDelay()
    {
        Player_Movement.Instance.isDisable = true;
        
        if (dreamSpace != null)
        {
            dreamSpace.SetActive(true);
        }

        if (checkpoint != null)
        {
            checkpoint.GetComponent<Collider2D>().enabled = false;
            checkpoint.GetComponent<Checkpoint_Interaction>().enabled = false;
            checkpoint.GetComponent<FOD_Agent>().enabled = false;
        }
        
        if (hintToShow != null)
        {
            hintToShow.SetActive(true);
        }

        if (hintToHide != null)
        {
            hintToHide.SetActive(false);
        }
        
        yield return new WaitForSeconds(0.5f);
        Player_Movement.Instance.isDisable = false;
    }
    
}
