using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding_Manager : MonoBehaviour
{
    public static PathFinding_Manager Instance;
    
    [Header("Layer Settings")]
    public LayerMask wallLayer;
    public LayerMask boxLayer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public List<Vector3> CalculateAStarPath(Vector3 start, Vector3 goal, HashSet<Vector3> recentPositions = null)
    {
        PriorityQueue<Vector3> openSet = new PriorityQueue<Vector3>();
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
        Dictionary<Vector3, float> gScore = new Dictionary<Vector3, float> { { start, 0 } };
        Dictionary<Vector3, float> fScore = new Dictionary<Vector3, float> { { start, Vector3.Distance(start, goal) } };
        
        openSet.Enqueue(start, fScore[start]);
        cameFrom[start] = start;
        
        while (openSet.Count > 0)
        {
            Vector3 current = openSet.Dequeue();
            if (current == goal) return ReconstructPath(cameFrom, current);
            
            foreach (Vector3 neighbor in GetNeighbors(current))
            {
                if (recentPositions != null && recentPositions.Contains(neighbor)) continue;

                float repeatPenalty = recentPositions != null && recentPositions.Contains(neighbor) ? 2.0f : 1.0f;
                float tentativeGScore = gScore[current] + Vector3.Distance(current, neighbor) * repeatPenalty;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Vector3.Distance(neighbor, goal);
                    openSet.Enqueue(neighbor, fScore[neighbor]);
                }
            }
        }
        return null;
    }

    private List<Vector3> GetNeighbors(Vector3 position)
    {
        Vector3[] possibleMoves = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        List<Vector3> neighbors = new List<Vector3>();

        foreach (Vector3 move in possibleMoves)
        {
            Vector3 targetPosition = position + move;
            if (!Physics2D.OverlapPoint(targetPosition, wallLayer) && !Physics2D.OverlapPoint(targetPosition, boxLayer))
            {
                neighbors.Add(targetPosition);
            }
        }
        return neighbors;
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
}