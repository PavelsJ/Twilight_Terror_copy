using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mimic_Movement : MonoBehaviour, IEnemy, IInteractable
{
    private Coroutine coroutine;
    
    [Header("Mimic Settings")]
    public float speed = 5f;
    public Transform movePoint;
    
    private GameObject currentTarget;
    
    private bool isActive = false;
    private bool isMoving = false;
    
    [Header("Scene Interaction Objects")]
    public List<GameObject> switchParts = new List<GameObject>();
    public bool hasSwitchedPart = false;
    
    private GameObject lastSwitchedPart = null;
    
    [Header("Compounds")]
    public GameObject[] mimicParts;
    public SpriteMask spriteMask;
    public GameObject bloodSplash;
    private FOD_Agent agent;
    
    private void OnEnable()
    {
        if (Player_Movement_Manager.Instance != null)
        {
            Player_Movement_Manager.Instance.RegisterEnemy(this);
        }
    }
    
    private void OnDisable()
    {
        if (Player_Movement_Manager.Instance != null)
        {
            Player_Movement_Manager.Instance.DeregisterEnemy(this);
        }
    }
    
    public void ActivateMimic()
    { 
        isActive = true;
    }
    
    void Start()
    {
        movePoint.parent = PathFinding_Manager.Instance.transform;
        agent = GetComponent<FOD_Agent>();

        foreach (var t in mimicParts)
        {
            t.transform.parent = null;
        }
    }
    
    public void OnPlayerMoved()
    {
        MoveTowardsTarget();
        isMoving = true;
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
   
    private void MoveTowardsTarget()
    {
        if (switchParts == null || switchParts.Count == 0)
        {
            Debug.Log("No switch parts available. Cannot move.");
            return;
        }
        
        if (currentTarget == null)
        {
            currentTarget = GetRandomSwitchPart();
        }

        if (currentTarget != null)
        {
            List<Vector3> path = PathFinding_Manager.Instance.FindPath(movePoint.position, currentTarget.transform.position);
            
            if (path != null && path.Count > 1)
            {
                Vector3 nextPosition = path[1];

                movePoint.position = nextPosition;
            }
        }
    }
    
    private GameObject GetRandomSwitchPart()
    {
        GameObject selected = switchParts[Random.Range(0, switchParts.Count)];
        
        if (selected == lastSwitchedPart)
        {
            foreach (var part in switchParts)
            {
                if (part != lastSwitchedPart)
                {
                    selected = part;
                    break;
                }
            }
        }
        
        lastSwitchedPart = selected;
        return selected;
    }
    
    public void RemoveSwitchPart(GameObject part)
    {
        if (switchParts.Contains(part))
        {
            switchParts.Remove(part);

            if (switchParts.Count == 0)
            {
                StopChasing();
            }
            else
            {
                ResetTarget();
            }
        }
    }
    
    private void StopChasing()
    {
        currentTarget = null;
        
        isMoving = false;
        isActive = false;

        DestroyObject();
    }

    
    public void ResetTarget()
    {
        currentTarget = null;
        hasSwitchedPart = !hasSwitchedPart;
    }
    
    public void DestroyObject()
    {
        // bloodSplash.SetActive(true);
        // bloodSplash.transform.parent = null;
        // bloodSplash.transform.rotation = Quaternion.Euler(Vector3.zero);
        
        Player_Movement_Manager.Instance.DeregisterEnemy(this);
        
        FOD_Agent agent = gameObject.GetComponent<FOD_Agent>();
        agent.deactivateOnEnd = true;
        agent.EndAgent();
        
        foreach (var t in mimicParts)
        {
            t.SetActive(false);
        }
    }
}
