using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spider_Chase : Enemy_Spider_Movement
{
    [Header("Spider Chase")]
    public bool isChasingPlayer = false;
    public Transform player;
    
    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    
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
        List<Vector3> path = CalculatePath(movePoint.position, player.position);
        
        if (path != null && path.Count > 1) 
        {
            pathQueue.Clear();
            for (int i = 1; i < path.Count; i++) 
            {
                pathQueue.Enqueue(path[i]);
            }

            if (pathQueue.Count > 0)
            {
                movePoint.position = pathQueue.Dequeue();
            }
        }
    }

    private void PatrolMovement()
    {
        if (CanMove(currentDirection))
        {
            movePoint.position += currentDirection;
        }
        else
        {
            currentDirection = -currentDirection;
            if (CanMove(currentDirection))
            {
                movePoint.position += currentDirection;
            }
        }
    }

    private List<Vector3> CalculatePath(Vector3 start, Vector3 goal)
    {
        Queue<Vector3> frontier = new Queue<Vector3>();
        frontier.Enqueue(start);

        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            Vector3 current = frontier.Dequeue();

            if (current == goal)
            {
                return ReconstructPath(cameFrom, current);
            }

            foreach (Vector3 neighbor in GetNeighbors(current))
            {
                if (!cameFrom.ContainsKey(neighbor))
                {
                    frontier.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        return null; // Если пути нет
    }

    private List<Vector3> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 current)
    {
        List<Vector3> path = new List<Vector3> { current };
        while (cameFrom[current] != current)
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }

    private List<Vector3> GetNeighbors(Vector3 position)
    {
        Vector3[] possibleMoves = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        List<Vector3> neighbors = new List<Vector3>();

        foreach (Vector3 move in possibleMoves)
        {
            Vector3 targetPosition = position + move;
            if (!Physics2D.OverlapPoint(targetPosition, boxLayer) && !Physics2D.OverlapPoint(targetPosition, wallLayer))
            {
                neighbors.Add(targetPosition);
            }
        }

        return neighbors;
    }
    
    private bool CanMove(Vector3 direction)
    {
        Vector3 targetPosition = movePoint.position + direction;
        return !Physics2D.OverlapPoint(targetPosition, boxLayer) && !Physics2D.OverlapPoint(targetPosition, wallLayer);
    }
}
