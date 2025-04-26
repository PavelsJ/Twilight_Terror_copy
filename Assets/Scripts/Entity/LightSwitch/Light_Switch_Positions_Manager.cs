using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_Switch_Positions_Manager : MonoBehaviour
{
    public Transform[] possiblePositions;
   
    private Transform currentPosition;
    private Transform previousPosition;
    
    private HashSet<Transform> occupiedPositions = new HashSet<Transform>();
    
    public Transform RandomizePosition(Transform occupiedPosition)
    {
        if (possiblePositions.Length == 0) return occupiedPosition;

        Transform randomPosition;
        int safety = 0;

        do
        {
            int randomIndex = Random.Range(0, possiblePositions.Length);
            randomPosition = possiblePositions[randomIndex];
            safety++;

            if (safety > 20)
            {
                Debug.LogWarning("Too many attempts — fallback to currentPosition");
                return currentPosition != null ? currentPosition : occupiedPosition;
            }

        } while (
            randomPosition == previousPosition ||
            randomPosition == occupiedPosition ||
            occupiedPositions.Contains(randomPosition) ||
            randomPosition == currentPosition
        );

        previousPosition = currentPosition;
        currentPosition = randomPosition;

        return currentPosition;
    }

    public void OccupyPosition(Transform occupiedPosition)
    {
        if (occupiedPosition != null && !occupiedPositions.Contains(occupiedPosition))
        {
            occupiedPositions.Add(occupiedPosition);
        }
    }
    
    public void ReleasePosition(Transform occupiedPosition)
    {
        if (occupiedPosition != null && occupiedPositions.Contains(occupiedPosition))
        {
            occupiedPositions.Remove(occupiedPosition);
        }
    }
}
