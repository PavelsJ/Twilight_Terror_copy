using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint_Interaction : MonoBehaviour
{
    public float invulnerabilityDistance = 2f;
    public float enemyCheckRadius = 3f;
    public LayerMask enemyLayer;
    
    private bool isActive = false;
    private bool isInvincible = false;
    private bool enemyTooClose = false;
    
    public Fire_Fly_Interation fire;
    private Transform player;
    private FOD_Agent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<FOD_Agent>();
        animator = GetComponent<Animator>();

        if (agent != null )
        {
            agent.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            player = other.transform;
            ActivateCheckpoint();
        }
    }

    private void Update()
    {
        if (isActive && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            bool isWithinRange = distanceToPlayer <= invulnerabilityDistance;
            
            if (isWithinRange != isInvincible)
            {
                isInvincible = isWithinRange;
                Player_Movement_Manager.Instance.SetInvulnerability(isInvincible);
            }
        }
        
        if (isActive && agent != null)
        {
            Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, enemyCheckRadius, enemyLayer);
            bool enemyNearby = nearbyEnemies.Length > 0;

            if (enemyNearby && !enemyTooClose)
            {
                agent.SetMinRadiusValue();
                enemyTooClose = true;
            }
            else if (!enemyNearby && enemyTooClose)
            {
                agent.SetMaxRadiusValue();
                enemyTooClose = false;
            }
        }
    }

    private void ActivateCheckpoint()
    {
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.LightSource);
        
        isActive = true;
        animator.SetTrigger("Light");
        
        if (agent != null)
        {
            if (agent.enabled)
            {
                agent.ActivateAgent();
            }
            else
            {
                agent.enabled = true;
            }
        }
        
        if (fire != null)
        {
            fire.Deactivate();
        }
        
        Debug.Log("Checkpoint Activated!");
    }
    
    public void DeactivateCheckpoint()
    {
        isActive = false;
        isInvincible = false;
        enemyTooClose = false;
        
        player = null;

        if (agent != null)
        {
            agent.EndAgent();
        }
        
        if (fire != null)
        {
            fire.Activate();
        }

        if (Player_Movement_Manager.Instance != null)
        {
            Player_Movement_Manager.Instance.SetInvulnerability(false);
        }

        Debug.Log("Checkpoint Deactivated!");
    }

    public bool IsActive()
    {
        return isActive;
    }
}
