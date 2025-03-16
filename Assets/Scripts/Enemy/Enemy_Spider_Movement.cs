using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spider_Movement : MonoBehaviour, IEnemy, IInteractable
{
    [Header("Spider Settings")]
    public Vector2 firstDirection = Vector2.right;

    internal Vector3 currentDirection;
    
    public float speed = 5;
    public bool isMoving = false;
    
    [Header("Transform References")] 
    public Transform movePoint; 
    
    [Header("Layer Settings")] 
    public LayerMask wallLayer;
    public LayerMask boxLayer;
    
    [Header("Compounds")]
    public SpriteMask spriteMask;
    public GameObject bloodSplash;
    
    void Start()
    {
        Player_Movement_Manager.Instance.RegisterEnemy(this);

        if (movePoint != null)
        {
            movePoint.parent = null;
        }
        
        currentDirection = firstDirection;
        
        spriteMask.enabled = false;
    }
    
    void Update()
    {
        if (isMoving && movePoint != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
            {
                isMoving = false; 
            }
        }
    }
    
    public virtual void OnPlayerMoved()
    {
        if (isMoving || movePoint == null) return;
        
        PatrolMovement();
        
        isMoving = true;
    }

    internal void PatrolMovement()
    {
        if (CanMove(currentDirection))
        {
            movePoint.position += currentDirection;
        }
        else
        {
            currentDirection = -currentDirection;
            if (CanMove(currentDirection))
            {
                movePoint.position += currentDirection;
            }
        }
    }
    
    private bool CanMove(Vector3 direction)
    {
        Vector3 targetPosition = movePoint.position + direction;
        return !Physics2D.OverlapPoint(targetPosition, boxLayer) && !Physics2D.OverlapPoint(targetPosition, wallLayer);
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
