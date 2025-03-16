using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Centipede_Movement : MonoBehaviour, IEnemy
{
    [Header("Centipede Settings")]
    public float speed = 5f;
    public bool isMoving = false;
  
    [Header("Transform References")] 
    public Transform player;
    private Transform target;
    
    public Transform movePoint;
    
    [Header("Layer Settings")] 
    public LayerMask wallLayer;
    
    [Header("Movement Settings")]
    public float detectionRange = 2f;
    public SpriteMask spriteMask;
    
    [Header("Compounds")]
    public Copy_Past_Movement copy;
    
    private readonly Queue<Vector3> recentPositions = new Queue<Vector3>(); 
    private const int recentPositionLimit = 3;
    
    void Start()
    {
        Player_Movement_Manager.Instance.RegisterEnemy(this);
        spriteMask.enabled = false; 
        
        if (movePoint != null)
        {
            movePoint.parent = null; 
        }
    }

    void Update()
    {
        if (isMoving && movePoint != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
            
            Vector3 direction = movePoint.position - transform.position;
            if (direction != Vector3.zero)
            {
                RotateSprite(direction); 
            }
            
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
            {
                isMoving = false;
            }

            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                spriteMask.enabled = distanceToPlayer <= detectionRange;
            }

            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                
                spriteMask.enabled = distanceToPlayer <= detectionRange;
            }
        }
    }
    
    private void RotateSprite(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); 
    }

    public void OnPlayerMoved()
    {
        if (isMoving || movePoint == null) return;

        target = player;
        if (target != null)
        {
            List<Vector3> path = PathFinding_Manager.Instance.CalculateAStarPath(movePoint.position, target.position, new HashSet<Vector3>(recentPositions));
            if (path != null && path.Count > 1)
            {
                copy.UpdateSegmentPosition(movePoint.position);
                movePoint.position = path[1];

                recentPositions.Enqueue(movePoint.position);
                if (recentPositions.Count > recentPositionLimit)
                {
                    recentPositions.Dequeue();
                }

                isMoving = true;
            }
        }
    }
}