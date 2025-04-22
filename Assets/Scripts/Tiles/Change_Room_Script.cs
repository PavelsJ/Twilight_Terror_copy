using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_Room_Script : MonoBehaviour
{
    public int nextRoomIndex;
    public float transitionTime = 1.6f;
    
    public Grid_Manager gridInteraction;
    internal bool isActive = false;
    internal Transform player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isActive)
        { 
            player = other.transform;
            isActive = true;
            ChangeRoom();
        }
    }

    protected virtual void ChangeRoom()
    {
        gridInteraction.OnActive(nextRoomIndex, transitionTime);
            
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.Shake);
            
        StartCoroutine(DisableDelay());
    }

    internal IEnumerator DisableDelay()
    {
        Player_Movement.Instance.isDisable = true;
        yield return new WaitForSeconds(transitionTime + 0.1f);
        Player_Movement.Instance.isDisable = false;
    }
}
