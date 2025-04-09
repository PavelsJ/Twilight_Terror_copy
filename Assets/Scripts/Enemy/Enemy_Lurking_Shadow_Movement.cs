using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Lurking_Shadow_Movement : MonoBehaviour, IEnemy, IInteractable
{
    [Header("Shadow Settings")]
    public float chaseDistance = 3.5f;
    
    public float speed = 5;
    public bool isNear = false;
    
    private bool isMoving = false;
    private int moveCooldown = 0;
    
    [Header("Transform References")] 
    public Transform movePoint; 
    public Transform player;
    
    [Header("Compounds")]
    public SpriteMask spriteMask;
    public GameObject bloodSplash;
    private FOD_Agent fodAgent; 
    
    private readonly Queue<Vector3> recentPositions = new Queue<Vector3>(); 
    private const int recentPositionLimit = 3;
    
    void Start()
    {
        Player_Movement_Manager.Instance.RegisterEnemy(this);

        if (movePoint != null)
        {
            
            movePoint.parent = PathFinding_Manager.Instance.transform;
        }
        
        fodAgent = player.GetComponent<FOD_Agent>();
        spriteMask.enabled = false;
    }
    
    public void OnPlayerMoved()
    {
        if (isMoving || movePoint == null) return;

        if (isNear)
        {
            moveCooldown++;

            if (moveCooldown % 2 == 0)
            {
                MoveTowardsTarget();
                isMoving = true;
            }
        }
    }
    
    private void Update()
    {
        CheckDistanceToPlayer();
        
        if (isMoving && movePoint != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
            {
                isMoving = false; 
            }
        }
        
        if (!isNear)
        {
            moveCooldown = 0;
        }
    }
    
    private void CheckDistanceToPlayer()
    {
        if (player == null) return;

        float sqrDistance = (player.position - transform.position).sqrMagnitude;
        bool isWithinRange = sqrDistance <= chaseDistance * chaseDistance;

        if (isWithinRange != isNear)
        {
            isNear = isWithinRange;
            spriteMask.enabled = isNear;

            if (fodAgent != null)
            {
                if (isNear)
                {
                    Music_Manager.instance.PlaySound(Music_Manager.SoundType.Noise);
                    Music_Manager.instance.EnterEnemyEncounter();
                    fodAgent.SetMinRadiusValue();
                }
                else
                {
                    Music_Manager.instance.ExitEnemyEncounter();
                    fodAgent.SetMaxRadiusValue();
                }
            }
        }
    }
    
    private void MoveTowardsTarget()
    {
        List<Vector3> path = PathFinding_Manager.Instance.FindPath(movePoint.position, player.position);
        if (path != null && path.Count > 1)
        {
            movePoint.position = path[1];

            recentPositions.Enqueue(movePoint.position);
            if (recentPositions.Count > recentPositionLimit)
            {
                recentPositions.Dequeue();
            }

            isMoving = true;
        }
    }
    
    public void DestroyObject()
    {
        bloodSplash.SetActive(true);
        bloodSplash.transform.parent = null;
        bloodSplash.transform.rotation = Quaternion.Euler(Vector3.zero);
        
        Player_Movement_Manager.Instance.DeregisterEnemy(this);
        
        Destroy(gameObject);
    }
}
