using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Larvae_Movement : MonoBehaviour, IEnemy, IInteractable
{
    public float speed = 5;
    public bool isMoving = false;
    public bool isDead = false;
    
    [Header("Transform References")] 
    public Transform movePoint; 
    public Transform target;
    
    private System.Action<GameObject> onReachedEnd;
    
    [Header("Compounds")]
    public GameObject lightBulb;
    public GameObject bloodSplash;
    
    public void Init(Transform target, System.Action<GameObject> callback)
    {
        this.target = target;
        onReachedEnd = callback;
        
        isMoving = true;
        movePoint.position = transform.position;
        
        GetComponent<FOD_Agent>().enabled = true;
        
        if (Player_Movement_Manager.Instance != null)
        {
            Player_Movement_Manager.Instance.RegisterEnemy(this);
        }
    }
    void Start()
    {
        if (movePoint != null)
        {
            movePoint.parent = PathFinding_Manager.Instance.transform;
        }
    }
    
    private void OnDisable()
    {
        if (Player_Movement_Manager.Instance != null)
        {
            Player_Movement_Manager.Instance.DeregisterEnemy(this);
        }
    }
    
    void Update()
    {
        if (isMoving && movePoint != null && !isDead)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
            {
                isMoving = false; 
                
                if (Vector3.Distance(transform.position, target.position) <= 0.05f)
                {
                    onReachedEnd?.Invoke(gameObject);
                }
            }
        }
    }
    
    public void OnPlayerMoved()
    {
       MoveTowardsPos();
       isMoving = true;
    }
    
    private void MoveTowardsPos()
    {
        List<Vector3> path = PathFinding_Manager.Instance.FindPath(movePoint.position, target.position);
        if (path != null && path.Count > 1)
        {
            Vector3 nextPosition = path[1];
            
            if (nextPosition.x < transform.position.x)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y); 
            }
            else if (nextPosition.x > transform.position.x)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y); 
            }

            movePoint.position = nextPosition;
        }
    }

    public void DestroyObject()
    {
        bloodSplash.SetActive(true);
        bloodSplash.transform.parent = null;
        bloodSplash.transform.rotation = Quaternion.Euler(Vector3.zero);
        
        Instantiate(lightBulb, transform.position, Quaternion.identity);
        
        isDead = true;
        Player_Movement_Manager.Instance.DeregisterEnemy(this);
        FOD_Agent agent = gameObject.GetComponent<FOD_Agent>();
        if (agent != null)
        {
            agent.deactivateOnEnd = true;
            agent.EndAgent();
        }
    }
}
