using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spider_Chase : Enemy_Spider_Movement
{
    [Header("Spider Chase")]
    public bool isChasingPlayer = false;
    public Transform player;
    
    private readonly Queue<Vector3> recentPositions = new Queue<Vector3>(); 
    private const int recentPositionLimit = 3;
    
    public override void OnPlayerMoved()
    {
        if (isMoving || movePoint == null) return;
        
        if (isChasingPlayer && player != null)
        {
            MoveTowardsPlayer();
        }
        else
        {
            PatrolMovement();
        }

        isMoving = true;
    }
    
    private void MoveTowardsPlayer()
    {
        List<Vector3> path = PathFinding_Manager.Instance.CalculateAStarPath(movePoint.position, player.position, new HashSet<Vector3>(recentPositions));
        if (path != null && path.Count > 1)
        {
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
