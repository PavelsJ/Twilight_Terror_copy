using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mimic_Movement : MonoBehaviour, IEnemy, IInteractable
{
    private Coroutine coroutine;
    
    [Header("Mimic Settings")]
    public float speed = 5f;
    public Transform movePoint;
    
    private bool isActive = false;
    private bool isMoving = false;
    
    private bool isBlocking = false;
    public bool isSwapping = false;
    
    private GameObject currentTarget;
    private Transform playerTransform;
    
    [Header("Scene Interaction Objects")]
    public List<GameObject> switchParts = new List<GameObject>();
    
    private GameObject lastSwitchedPart = null;
    private GameObject lastNearestSwitch = null;
    
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
        playerTransform = Player_Movement.Instance.movePoint.transform;
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
        if (isActive)
        {
            MoveTowardsTarget();
            isMoving = true;
        }
    }
    
    void Update()
    {
        if(!isActive) return;
        
        if (isMoving && movePoint != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
            {
                isMoving = false; 
            }
        }
        
        TrySwapWithPlayer();
    }
   
    private void MoveTowardsTarget()
    {
        if (switchParts == null || switchParts.Count == 0)
            return;
        
        if (currentTarget == null)
            currentTarget = GetRandomSwitchPart();
    
        if (currentTarget != null)
        {
            List<Vector3> path = PathFinding_Manager.Instance.FindPath(movePoint.position, currentTarget.transform.position);
        
            if (path != null && path.Count > 1)
            {
                movePoint.position = path[1];
            }
        }
    }
    
    private void TrySwapWithPlayer()
    {
        if (isSwapping) return; 

        GameObject nearestSwitch = GetNearestSwitchPart();
        if (nearestSwitch == null) return;

        float playerToSwitchDistance = Vector3.Distance(playerTransform.position, nearestSwitch.transform.position);

        if (playerToSwitchDistance <= 1.5f)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);

            isSwapping = true;
            
            Music_Manager.instance.PlaySound(Music_Manager.SoundType.Noise);
            Music_Manager.instance.PlaySound(Music_Manager.SoundType.Twilight);
            
            coroutine = StartCoroutine(SwapWithPlayerRoutine(playerTransform, movePoint.transform, 2f));
        }
    }
    
    private IEnumerator SwapWithPlayerRoutine(Transform player, Transform mimic, float duration = 0.5f)
    {
        Collider2D playerCollider = Player_Movement.Instance.gameObject.GetComponent<Collider2D>();
        Player_Movement.Instance.isDisable = true;
        playerCollider.enabled = false;
        
        Vector3 playerStart = player.position;
        Vector3 mimicStart = mimic.position;
        Vector3 center = (playerStart + mimicStart) / 2f;

        float time = 0f;
        
        Vector3 offset = playerStart - center;
        float angle = Mathf.Atan2(offset.y, offset.x);

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            float theta = Mathf.Lerp(0f, Mathf.PI, t); 
            
            float radiusX = offset.magnitude;
            float radiusY = radiusX * 0.5f;
            
            Vector3 playerOffset = new Vector3(Mathf.Cos(angle + theta) * radiusX, Mathf.Sin(angle + theta) * radiusY, 0);
            Vector3 mimicOffset = new Vector3(Mathf.Cos(angle + theta + Mathf.PI) * radiusX, Mathf.Sin(angle + theta + Mathf.PI) * radiusY, 0);

            player.position = center + playerOffset;
            mimic.position = center + mimicOffset;
            transform.position = mimic.position;

            yield return null;
        }
        
        player.position = mimicStart;
        mimic.position = playerStart;
        transform.position = playerStart;
        
        playerCollider.enabled = true;
        Player_Movement.Instance.isDisable = false;
    }
    
    private GameObject GetNearestSwitchPart()
    {
        GameObject nearest = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 playerPos = playerTransform.transform.position;

        foreach (var part in switchParts)
        {
            float distance = Vector3.Distance(playerPos, part.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearest = part;
            }
        }

        return nearest;
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
