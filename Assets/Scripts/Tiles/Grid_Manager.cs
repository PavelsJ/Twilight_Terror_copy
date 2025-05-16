using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FODMapping;
using UnityEngine;

public class Grid_Manager : MonoBehaviour
{
    public Transform firstSector;
    public Transform[] midSectors;
    public Transform lastSector;

    public Transform sectorPosParent;
    public Transform playerTargetPos;
    
    [Header("References")]
    public Shake_Camera_Manager shakeCamera;
    private FOD_Manager manager;
    
    private readonly List<SectorPosGroup> sectorGroups = new();
    private Vector2 firstPos;
    private int currentSector = 0;
    
    private float transitionDuration;
    private bool isStart = false;
    
    [Header("Definition")]
    private static readonly Vector2 DEFAULT_PLAYER_POS = new Vector2(-4.5f, 1.5f);
    private static readonly Vector2 DEFAULT_SECTOR_POS = Vector2.zero;
    
    private const float DEFAULT_MOVE_SPEED = 5f;
    private const float DEFAULT_SHAKE_INTENSITY = 0.5f;

    private void Awake()
    {
        for (int i = 0; i < midSectors.Length; i++)
        {
            ActivateSector(i, false);
        }
    }
    
    void Start()
    {
        manager = FindObjectOfType<FOD_Manager>(true);
        
        if (manager != null)
        {
            manager.gameObject.SetActive(true);
            manager.StartCoroutine(manager.EnableInstantly());
        }
        
        foreach (Transform child in sectorPosParent)
        {
            var group = new SectorPosGroup();

            if (child.childCount == 0)
            {
                group.sectorPos.Add(child);
            }
            else
            {
                group.sectorPos.AddRange(child.Cast<Transform>());
            }

            sectorGroups.Add(group);
        }
        
        firstPos = DEFAULT_SECTOR_POS;

        StartCoroutine(MovementCoroutine());
    }

    private IEnumerator MovementCoroutine()
    {
        yield return new WaitForSeconds(1.4f);
        isStart = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && isStart)
        {
            Player_Movement.Instance.MovePlayerTo(DEFAULT_PLAYER_POS);
            isStart = false;
        }
    }

    public void OnActive(int sectorPosIndex, float time)
    {
        if (sectorPosIndex + 1 >= sectorGroups.Count) return;
        
        StartCoroutine(MoveSectorsSimultaneously(sectorPosIndex, time));
    }
    
    private IEnumerator MoveSectorsSimultaneously(int index, float time)
    {
        Transform target = GetSectorPosition(index);
        
        StartCoroutine(MoveSector(lastSector, target.position, time));
        yield return new WaitForSeconds(0.4f);
        ActivateSector(index, true);
    }

    public void PlaceSectorsInstantly(int index, int choice = 0)
    {
        if (index + 1 >= sectorGroups.Count) return;
        
        Transform target = GetSectorPosition(index, choice);
        PlaceSector(lastSector, target.position);
    }

    private IEnumerator MoveSector(Transform sector, Vector2 targetPos, float moveDuration)
    {
        float elapsedTime = 0f;
        Vector2 direction = (targetPos - (Vector2)sector.position).normalized; 
       
        while (elapsedTime < moveDuration)
        {
            float shakeStrength = Mathf.SmoothStep(0f, DEFAULT_SHAKE_INTENSITY, elapsedTime / moveDuration);

            sector.position += (Vector3)(direction * DEFAULT_MOVE_SPEED * Time.deltaTime);
            shakeCamera.ShakeCamera(shakeStrength);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        PlaceSector(sector, targetPos);
        
        shakeCamera.ShakeCamera(0);
    }

    private void PlaceSector(Transform sector, Vector2 targetPos)
    {
        sector.position = targetPos;
        currentSector += 1;
    }

    public void ResetSectorsState()
    {
        var playerMovement = Player_Movement.Instance;
        GameObject player = playerMovement.gameObject;
        
        Player_Movement_Manager.Instance.enemy.gameObject.SetActive(false);
        
        if (player != null)
        {
            playerMovement.movePoint.position = playerTargetPos.position;
            player.transform.position = playerTargetPos.position;
            
            player.SetActive((true));
        }
        
        lastSector.position = firstPos;

        for (int i = 0; i < midSectors.Length; i++)
        {
            ActivateSector(i, false);
        }
    }

    public void ChangeSectorState(int index)
    {
        lastSector.position = GetSectorPosition(index).position;
        ActivateSector(index, true);
    }
    
    private void ActivateSector(int index, bool isActive)
    {
        midSectors[index].gameObject.SetActive(isActive);
    }
    
    private Transform GetSectorPosition(int index, int choice = 0)
    {
        return sectorGroups[index + 1].sectorPos[choice];
    }

    public List<Transform> GetSectorGroup(int index)
    {
        if (index < 0 || index >= sectorGroups.Count)
            return new List<Transform>();

        return new List<Transform>(sectorGroups[index + 1].sectorPos);
    }
    
}

[System.Serializable]
public class SectorPosGroup
{
    public List<Transform> sectorPos = new List<Transform>();
}

