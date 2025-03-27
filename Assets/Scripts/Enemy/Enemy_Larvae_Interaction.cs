using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Larvae_Interaction : MonoBehaviour, IEnemy
{
    public GameObject larvaePrefab;
    
    public Transform larvaeSpawn;
    public Transform larvaeFinish;
    
    public int spawnInterval = 3;
    private int turnCounter = 0;
    private Queue<GameObject> larvaePool = new Queue<GameObject>();

    void Start()
    {
        Player_Movement_Manager.Instance.RegisterEnemy(this);
        
        for (int i = 0; i < 10; i++)
        {
            GameObject larvae = Instantiate(larvaePrefab, larvaeSpawn.position, Quaternion.identity);
            larvae.SetActive(false);
            larvaePool.Enqueue(larvae);
        }
    }
    
    public void OnPlayerMoved()
    {
        turnCounter++;
        if (turnCounter % spawnInterval == 0)
        {
            SpawnLarvae();
        }
    }
    
    private void SpawnLarvae()
    {
        GameObject larvae = larvaePool.Dequeue();
        
        larvae.transform.position = larvaeSpawn.position;
        larvae.SetActive(true);
        
        larvae.GetComponent<Enemy_Larvae_Movement>().Init(larvaeFinish, OnLarvaeReachedEnd);
        
        larvaePool.Enqueue(larvae);
    }

    private void OnLarvaeReachedEnd(GameObject larvae)
    {
        larvae.GetComponent<FOD_Agent>().enabled = false;
        larvae.SetActive(false);
    }
}
