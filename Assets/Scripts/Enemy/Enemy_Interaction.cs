using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Interaction : MonoBehaviour
{
    private Enemy_Mimic_Movement mimicMovement;

    private void Awake()
    {
        mimicMovement = GetComponent<Enemy_Mimic_Movement>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            var switchPart = other.GetComponent<ISwitchPart>();
            if (switchPart != null)
            {
                switchPart.RandomizePosition();
                mimicMovement.ResetTarget(); 
            }
        }
    }
}
