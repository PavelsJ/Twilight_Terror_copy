using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Eye : MonoBehaviour
{
    public float maxOffset = 0.1f;
    
    private Transform playerTarget;
    
    void OnEnable()
    {
        playerTarget = Player_Movement.Instance.transform;
    }

    private void Update()
    {
        if (playerTarget == null) return;
        
        Vector3 direction = playerTarget.position - transform.position;
        direction.z = 0f; 
            
        Vector3 offset = Vector3.ClampMagnitude(direction.normalized * maxOffset, maxOffset);
        transform.localPosition = offset;
    }
}
