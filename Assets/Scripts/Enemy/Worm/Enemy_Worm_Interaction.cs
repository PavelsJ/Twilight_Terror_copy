using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Worm_Interaction : MonoBehaviour, IEnemy
{
    public GameObject wormPrefab;
    
    public Transform wormSpawn;
    public Transform wormFinish;
   
    public int movesAfterTail = 3;

    private GameObject lastWorm = null;
    private Queue<GameObject> wormPool = new Queue<GameObject>();
    
    void Start()
    {
        Player_Movement_Manager.Instance.RegisterEnemy(this);

        for (int i = 0; i < 5; i++)
        {
            GameObject worm = Instantiate(wormPrefab, wormSpawn.position, Quaternion.identity, transform);
            worm.SetActive(false);
            wormPool.Enqueue(worm);
        }
    }

    public void OnPlayerMoved()
    {
        if (lastWorm != null && lastWorm.activeInHierarchy)
        {
            var movement = lastWorm.GetComponent<Enemy_Worm_Movement>();
            if (movement == null) return;

            Vector3 tailPos = movement.GetTailPosition();
            float actualDistance = Vector3.Distance(tailPos, wormSpawn.position);

            if (actualDistance >= movesAfterTail)
            {
                SpawnWorm();
            }
        }
        else
        {
            SpawnWorm();
        }
    }

    private void SpawnWorm()
    {
        GameObject worm;
        if (wormPool.Count == 0)
        {
            worm = Instantiate(wormPrefab, wormSpawn.position, Quaternion.identity, transform);
        }
        else
        {
            worm = wormPool.Dequeue();
        }

        worm.transform.position = wormSpawn.position;
        worm.SetActive(true);

        var movementComp = worm.GetComponent<Enemy_Worm_Movement>();
        movementComp.Init(wormFinish, (obj) =>
        {
            worm.SetActive(false);
            wormPool.Enqueue(worm);
        });

        lastWorm = worm;
    }
}