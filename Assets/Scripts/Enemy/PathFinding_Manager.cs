using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding_Manager : MonoBehaviour
{
    public static PathFinding_Manager Instance;
    
    [Header("Layer Settings")]
    public LayerMask wallLayer;
    public LayerMask boxLayer;
    
    private Dictionary<Vector3, List<Vector3>> cachedPaths = new Dictionary<Vector3, List<Vector3>>();
    private HashSet<Vector3> boxPositions = new HashSet<Vector3>();
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    public List<Vector3> FindPath(Vector3 start, Vector3 goal)
    {
        if (cachedPaths.TryGetValue(start, out var cachedPath) && cachedPath.Count > 0 && cachedPath[^1] == goal)
        {
            return new List<Vector3>(cachedPath);
        }
        
        HashSet<Vector3> closedSet = new HashSet<Vector3>();
        PriorityQueue<Vector3> openSet = new PriorityQueue<Vector3>();
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
        Dictionary<Vector3, float> gScore = new Dictionary<Vector3, float> { { start, 0 } };
        Dictionary<Vector3, float> fScore = new Dictionary<Vector3, float> { { start, Vector3.Distance(start, goal) } };
        
        openSet.Enqueue(start, fScore[start]);
        
        while (openSet.Count > 0)
        {
            Vector3 current = openSet.Dequeue();
            
            if (current == goal)
            {
                List<Vector3> path = ReconstructPath(cameFrom, current);
                cachedPaths[start] = path;
                return path;
            }
            
            closedSet.Add(current);
            
            foreach (Vector3 neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor)) continue;
                
                float tentativeGScore = gScore[current] + Vector3.Distance(current, neighbor);
                
                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = tentativeGScore + Vector3.Distance(neighbor, goal);
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
            if (!Physics2D.OverlapPoint(targetPosition, wallLayer) && !IsBoxAtPosition(targetPosition))
            {
                neighbors.Add(targetPosition);
            }
        }
        return neighbors;
    }
    
    private List<Vector3> ReconstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 current)
    {
        List<Vector3> path = new List<Vector3> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }
    
    public void SetBoxPosition(Vector3 oldPos, Vector3 newPos)
    {
        boxPositions.Remove(oldPos);
        boxPositions.Add(newPos);
    }

    public void RegisterBox(Vector3 pos)
    {
        boxPositions.Add(pos);
    }

    public void UnregisterBox(Vector3 pos)
    {
        boxPositions.Remove(pos);
    }

    public bool IsBoxAtPosition(Vector3 pos)
    {
        return boxPositions.Contains(pos);
    }
    
    public void ClearCache()
    {
        cachedPaths.Clear();
    }
}