using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Motion : MonoBehaviour
{
    internal bool isMoving = false;
    
    [Header("Motion Settings")]
    public float amplitude = 0.1f;
    public float cycleDuration = 4f;

    internal float startY;
    private float timer;
    
    void Awake()
    {
        startY = transform.position.y;
        timer = Random.Range(0f, cycleDuration); 
    }
    
    void Update()
    {
        if (!isMoving)
        {
            timer += Time.deltaTime;
            float t = (Mathf.Sin((timer / cycleDuration) * 2 * Mathf.PI) + 1f) / 2f; 
            float yOffset = Mathf.SmoothStep(-amplitude, amplitude, t);
        
            Vector3 pos = transform.position;
            pos.y = startY + yOffset;
            transform.position = pos;
        }
    }

}
