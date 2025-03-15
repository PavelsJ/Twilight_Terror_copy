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
    public Checkpoint_Interaction[] checkpointInteractions;
    
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
        if (manager == null)
        {
            Debug.Log("FOD_Manager is empty");
            return;
        }

        if (checkpointInteractions.Length > 0)
        {
            foreach (Checkpoint_Interaction checkpoint in checkpointInteractions)
            {
                checkpoint.enabled = false;
            }
        }
        
        if (bedInteraction != null)
        {
            bedInteraction.endScene = true;
            Player_Movement_Manager.Instance.isInvulnerable = true;
        }
        
        GetComponent<SpriteRenderer>().sprite = sprite;
        manager.StartCoroutine(manager.DisableWithDelay());
    }
    
}
