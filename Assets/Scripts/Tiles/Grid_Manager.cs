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
    
    private List<Transform> sectorPos;
    private Vector2 firstPos;
    
    private float transitionDuration;
    
    private bool isActive = false;
    private bool isStart = true;

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
        
        sectorPos = sectorPosParent.GetComponentsInChildren<Transform>()
            .Where(t => t != sectorPosParent)
            .ToList();
        
        firstPos = new Vector2(0, 0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D) && isStart)
        {
            Player_Movement.Instance.MovePlayerTo(new Vector2(-4.5f, 1.5f));
            isStart = false;
        }
    }

    public void OnActive(int sectorPosIndex, float time)
    {
        StartCoroutine(MoveSectorsSimultaneously(sectorPosIndex, time));
    }
    
    private IEnumerator MoveSectorsSimultaneously(int index, float time)
    {
        StartCoroutine(MoveSector(lastSector, sectorPos[index + 1].position, time));
        yield return new WaitForSeconds(0.4f);
        ActivateSector(index, true);
    }

    private IEnumerator MoveSector(Transform sector, Vector2 targetPos, float moveDuration)
    {
        float moveSpeed = 5;
        float elapsedTime = 0f;
        Vector2 direction = (targetPos - (Vector2)sector.position).normalized; 
        float maxShakeIntensity = 0.5f;

        while (elapsedTime < moveDuration)
        {
            float shakeStrength = Mathf.SmoothStep(0f, maxShakeIntensity, elapsedTime / moveDuration);

            sector.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
            shakeCamera.ShakeCamera(shakeStrength);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        sector.position = targetPos;
        shakeCamera.ShakeCamera(0);
    }

    public void ResetSectorsState()
    {
        GameObject player = Player_Movement.Instance.gameObject;
        Player_Movement_Manager.Instance.enemy.gameObject.SetActive(false);
        
        if (player != null)
        {
            Player_Movement.Instance.movePoint.position = playerTargetPos.position;
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
        lastSector.position = sectorPos[index + 1].position;
        ActivateSector(index, true);
    }
    
    private void ActivateSector(int index, bool isActive)
    {
        midSectors[index].gameObject.SetActive(isActive);
    }
}
