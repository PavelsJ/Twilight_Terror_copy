using System;
using System.Collections;
using System.Collections.Generic;
using FODMapping;
using UnityEngine;

public class Light_Switch_Interaction : MonoBehaviour
{
    public bool isActive = false;
    public Sprite sprite;
    
    [Header("Compounds")]
    public Bed_Interaction bedInteraction;
    public Grid_Manager gridManager;
    
    public GameObject checkpoint;
    
    private FOD_Manager manager;
    private Transform player;

    private void Start()
    {
        manager = FindObjectOfType<FOD_Manager>(true);
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
        
        HandleBedInteraction();
        
        GetComponent<SpriteRenderer>().sprite = sprite;
        manager.RemoveAgentsGradually(true);
    }

    private IEnumerator DisableWithDelay()
    {
        Player_Movement.Instance.isDisable = true;

        if (checkpoint != null)
        {
            checkpoint.GetComponent<Collider2D>().enabled = false;
            checkpoint.GetComponent<Checkpoint_Interaction>().enabled = false;
            checkpoint.GetComponent<FOD_Agent>().enabled = false;
        }
        
        HandleHintsInteraction();
        
        yield return new WaitForSeconds(0.5f);
        
        gridManager.ResetSectorsState();
        Player_Movement.Instance.isDisable = false;
    }

    protected virtual void HandleHintsInteraction()
    {
        
    }

    private void HandleBedInteraction()
    {
        if (bedInteraction != null)
        {
            bedInteraction.endScene = true;
            Player_Movement_Manager.Instance.isInvulnerable = true;
        }
    }
}
