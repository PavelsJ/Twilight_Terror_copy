using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spider_Chase : Enemy_Spider_Movement
{
    [Header("Spider Chase")]
    public bool isChasingPlayer = false;
    public Transform player;
    
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
        List<Vector3> path = PathFinding_Manager.Instance.FindPath(movePoint.position, player.position);
        
        if (path != null && path.Count > 1)
        {
            movePoint.position = path[1];
            
            isMoving = true;
        }
    }
}
