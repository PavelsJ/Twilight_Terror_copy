using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_Last_Sector : MonoBehaviour
{
    public int sectorIndex;
    private List<Transform> sectorPositions = new List<Transform>();
    
    public Grid_Manager gridManager;
    public Transform player;
    
    private bool isActive = false;

    private void OnEnable()
    {
        foreach (var child in gridManager.GetSectorGroup(sectorIndex))
        {
            sectorPositions.Add(child.transform);
        }
        
        isActive = true;
    }

    void LateUpdate()
    {
        if (isActive && sectorPositions.Count > 0)
        {
            float minDistance = float.MaxValue;
            int closestIndex = 0;

            for (int i = 0; i < sectorPositions.Count; i++)
            {
                float distance = Vector2.Distance(player.position, sectorPositions[i].position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }
            
            gridManager.PlaceSectorsInstantly(sectorIndex, closestIndex);
        }
    }

    public void ToggleSnapping()
    {
        isActive = !isActive;
    }
}
