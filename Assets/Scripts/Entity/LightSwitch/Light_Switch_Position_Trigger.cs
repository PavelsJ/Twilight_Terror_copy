using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_Switch_Position_Trigger : MonoBehaviour
{
    private Light_Switch_Positions_Manager manager;

    private void Start()
    {
        manager = transform.parent.GetComponent<Light_Switch_Positions_Manager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            manager.OccupyPosition(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            manager.ReleasePosition(transform);
        }
    }
}
