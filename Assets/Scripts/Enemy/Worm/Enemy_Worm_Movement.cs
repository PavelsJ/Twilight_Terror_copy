using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Worm_Movement : MonoBehaviour, IEnemy
{
    public GameObject wormHeadPrefab;
    public GameObject wormBodyPrefab;
    public GameObject wormTailPrefab;
    public int minSegments = 2;
    public int maxSegments = 4;

    public float stepInterval = 1f; 
    private int targetSegments;
    
    private bool isActive = false;
    private bool isGrowing = true;
    private bool isFinished = false;
    
    private Vector3 finalPosition;

    private List<Vector3> headHistory = new List<Vector3>();
    private List<Enemy_Segment> segments = new List<Enemy_Segment>();

    private Transform target;
    private System.Action<GameObject> onReachedEnd;

    public void Init(Transform target, System.Action<GameObject> callback)
    {
        this.target = target;
        this.onReachedEnd = callback;
        
        ResetState();

        targetSegments = Random.Range(minSegments, maxSegments + 1);
        Vector3 startPos = transform.position;
        headHistory.Add(startPos); 

        if (Player_Movement_Manager.Instance != null)
            Player_Movement_Manager.Instance.RegisterEnemy(this);
    }
    
    private void ResetState()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        segments.Clear();
        headHistory.Clear();
        isGrowing = true;
        isActive = true;
        isFinished = false;
    }
    
     public void OnPlayerMoved()
    {
        if (!isActive) return;

        UpdateHeadPosition();
        AddSegmentsIfGrowing();
        MoveSegments();
        RemoveSegmentsIfFinished();
        CompleteWormMovement();

        LimitHistoryLength();
    }

    private void UpdateHeadPosition()
    {
        if (isFinished)
        {
            headHistory.Insert(0, finalPosition);
        }
        else
        {
            Vector3 currentHeadPos = headHistory[0];
            List<Vector3> path = PathFinding_Manager.Instance.FindPath(currentHeadPos, target.position);
            if (path == null || path.Count < 2) return;

            Vector3 nextHeadPos = path[1];
            headHistory.Insert(0, nextHeadPos);

            if (Vector3.Distance(nextHeadPos, target.position) < 0.1f)
            {
                isFinished = true;
                finalPosition = nextHeadPos;
            }
        }
    }

    private void AddSegmentsIfGrowing()
    {
        if (isGrowing && segments.Count < targetSegments)
        {
            int requiredHistory = Mathf.RoundToInt((segments.Count + 1) * stepInterval);
            if (headHistory.Count > requiredHistory)
            {
                AddSegmentAtIndex(requiredHistory);
            }
        }
    }

    private void MoveSegments()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            int index = Mathf.RoundToInt((i + 1) * stepInterval);
            if (index < headHistory.Count)
            {
                segments[i].MoveTo(headHistory[index]);
            }
        }
    }

    private void RemoveSegmentsIfFinished()
    {
        if (isFinished && segments.Count > 0)
        {
            Enemy_Segment firstSegment = segments[0];
            if (Vector3.Distance(firstSegment.transform.position, finalPosition) < 0.05f && !firstSegment.isMoving)
            {
                Destroy(firstSegment.gameObject);
                segments.RemoveAt(0);
            }
        }
    }

    private void CompleteWormMovement()
    {
        if (isFinished && segments.Count == 0)
        {
            isActive = false;
            onReachedEnd?.Invoke(gameObject);
            Player_Movement_Manager.Instance?.DeregisterEnemy(this);
        }
    }

    private void LimitHistoryLength()
    {
        int maxHistory = Mathf.RoundToInt((targetSegments + 2) * stepInterval);
        if (headHistory.Count > maxHistory)
            headHistory.RemoveAt(headHistory.Count - 1);
    }

    private void AddSegmentAtIndex(int index)
    {
        GameObject prefab = GetSegmentPrefab();
        Vector3 pos = headHistory[index];
        GameObject segment = Instantiate(prefab, pos, Quaternion.identity, transform);
        segments.Add(segment.GetComponent<Enemy_Segment>());

        if (segments.Count >= targetSegments)
            isGrowing = false;
    }

    private GameObject GetSegmentPrefab()
    {
        if (segments.Count == 0) return wormHeadPrefab;
        if (segments.Count == targetSegments - 1) return wormTailPrefab;
        return wormBodyPrefab;
    }

    public Vector3 GetTailPosition()
    {
        if (segments.Count > 0)
            return segments[^1].transform.position;
        return transform.position;
    }
    
    private void OnDisable()
    {
        Player_Movement_Manager.Instance?.DeregisterEnemy(this);
    }
}
