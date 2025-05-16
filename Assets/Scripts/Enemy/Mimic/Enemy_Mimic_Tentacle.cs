using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mimic_Tentacle : MonoBehaviour, IEnemy
{
    public GameObject tentacle;
    
    private int count;
    private Vector3 initialPosition; 
    
    private Collider2D tentacleCollider;
    private Animator animator;

    private void Start()
    {
        if (tentacle != null)
        {
            initialPosition = transform.position;
            tentacle.transform.position = initialPosition;
            
            tentacleCollider = tentacle.GetComponent<Collider2D>();
        }
        
        animator = GetComponent<Animator>();
        
        if (Player_Movement_Manager.Instance != null)
        {
            Player_Movement_Manager.Instance.RegisterEnemy(this);
        }
    }
    
    // private void OnEnable()
    // {
    //     if (Player_Movement_Manager.Instance != null)
    //     {
    //         Player_Movement_Manager.Instance.RegisterEnemy(this);
    //     }
    // }
    
    private void OnDisable()
    {
        if (Player_Movement_Manager.Instance != null)
        {
            Player_Movement_Manager.Instance.DeregisterEnemy(this);
        }
    }
    
    public void OnPlayerMoved()
    {
        count++;
        
        if (count % 4 == 1)
        {
            SpawnTentacle();
        }
        else if (count % 4 == 3)
        {
            DespawnTentacle();
        }
    }

    private void SpawnTentacle()
    {
        animator.SetTrigger("Up");
        StartCoroutine(CreateTentacle());
    }

    private IEnumerator CreateTentacle()
    {
        yield return new WaitForSeconds(0.5f);
        tentacleCollider.enabled = true;
    }

    private void DespawnTentacle()
    {
        animator.SetTrigger("Down");
        tentacleCollider.enabled = false;
    }
}
