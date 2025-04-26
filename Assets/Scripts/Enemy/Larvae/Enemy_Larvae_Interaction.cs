using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Larvae_Interaction : MonoBehaviour, IEnemy
{
    public GameObject larvaeBlackPrefab;
    public GameObject larvaeWhitePrefab;
    
    public Transform larvaeSpawn;
    public Transform larvaeFinish;
    
    public int spawnInterval = 3;
    private int turnCounter = 0;
    private int larvaeTypeCounter = 0;
    
    private Queue<GameObject> larvaePool = new Queue<GameObject>();

    void Start()
    {
        Player_Movement_Manager.Instance.RegisterEnemy(this);
        
        for (int i = 0; i < 10; i++)
        {
            GameObject prefabToUse = (i % 2 == 0) ? larvaeWhitePrefab : larvaeBlackPrefab;
            GameObject larvae = Instantiate(prefabToUse, larvaeSpawn.position, Quaternion.identity, transform);
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
        GameObject larvae;
        
        if (larvaePool.Count == 0)
        {
            GameObject prefabToUse = (larvaeTypeCounter % 2 == 0) ? larvaeWhitePrefab : larvaeBlackPrefab;
            larvae = Instantiate(prefabToUse, larvaeSpawn.position, Quaternion.identity, transform);
        }
        else
        {
            larvae = larvaePool.Dequeue();
        }

        larvaeTypeCounter++;

        larvae.transform.position = larvaeSpawn.position;
        larvae.SetActive(true);

        larvae.GetComponent<Enemy_Larvae_Movement>().Init(larvaeFinish, OnLarvaeReachedEnd);

        larvaePool.Enqueue(larvae);
    }

    private void OnLarvaeReachedEnd(GameObject larvae)
    {
        FOD_Agent agent = larvae.GetComponent<FOD_Agent>();
        if (agent != null)
        {
            agent.deactivateOnEnd = true;
            agent.EndAgent();
        }
    }
}
