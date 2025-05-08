using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Copy_Mimic_Movement : MonoBehaviour
{
    public Transform target;
    public float distance = 0.1f;
    public float speed = 5f;
    
    
    void Update()
    {
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            
            if (distanceToTarget > distance)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            }
        }
    }
}
