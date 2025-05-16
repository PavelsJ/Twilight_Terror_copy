using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mimic_Boss : MonoBehaviour, IEnemy
{
    private enum BossState { Normal, Charging}
    private BossState currentState = BossState.Normal;

    [Header("Transform References")] 
    public Transform player;
    public Transform movePoint;
    
    [Header("Boss Settings")]
   
    public int speed = 5;
    
    public int stepsBetweenSimpleAttack = 5;
    public int stepsBetweenTentacleAttack = 10;
    
    private int stepsTaken = 0;
    private int closeAttackCounter = 0;
    
    public bool isActive = false;
    public bool isMoving = false;

    [Header("Boss Attack")] 
    public float distanceToPlayer;
    private float currentDistance;
    
    private bool isNear = false;
    private bool isShaking = false;
    private bool isCharging = false;
    
    private Vector3 originalPosition;
    
    [Header("Tentacle Settings")]
    public GameObject tentacle;
    public Transform tentacleParent;
    public LayerMask wallLayer;
    
    public int tentacleActivationRadius = 3;
    
    public int tentaclesPerAttackMin = 3;
    public int tentaclesPerAttackMax = 4;
    
    [Header("Compounds")]
    public Shake_Camera_Manager shakeCameraManager;
    
    private void Start()
    {
        if (movePoint != null)
        {
            movePoint.parent = PathFinding_Manager.Instance.transform; 
        }
    }
    
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
    
    private void Update()
    {
        if (!isActive ) return;

        if (isMoving && movePoint != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, movePoint.position) <= 0.05f)
            {
                isMoving = false;
            }
        }

        if (isCharging)
        {
            if (!isShaking)
            {
                isShaking = true;
            }

            float shakeIntensity = 0.03f;
            transform.position = originalPosition + (Vector3)(Random.insideUnitCircle * shakeIntensity);
        }
        else if (isShaking)
        {
            transform.position = originalPosition;
            isShaking = false;
        }
    }

    public void OnCutscene()
    {
        isActive = true;
    }

    public void OnPlayerMoved()
    {
        if (!isActive) return;

        if (!isMoving && movePoint != null)
        {
            currentDistance = Vector3.Distance(transform.position, player.position);
            isNear = currentDistance <= distanceToPlayer;
            
            switch (currentState)
            {
                case BossState.Normal:
                    
                    closeAttackCounter++;

                    if (isNear && closeAttackCounter >= stepsBetweenSimpleAttack)
                    {
                        currentState = BossState.Charging;
                        originalPosition = movePoint.position;
                        
                        isCharging = true;
                        
                        return;
                    }

                    Movement();
                    break;

                case BossState.Charging:
                    
                    isCharging = false;
                    
                    StartCoroutine(StartAttack());
                    
                    currentState = BossState.Normal;
                    closeAttackCounter = 0;
                    return;
            }
        }
    }
    
    private void Movement()
    {
        Vector3 moveDir = Vector3.right;
        float yDelta = player.position.y - transform.position.y;

        if (Mathf.Abs(yDelta) > 0.1f)
        {
            moveDir.y = Mathf.Sign(yDelta);
        }

        movePoint.position += new Vector3(moveDir.x, moveDir.y, 0);
        stepsTaken++;

        if (stepsTaken % stepsBetweenTentacleAttack == 0)
        {
            StartCoroutine(SpawnTentacles());
        }

        isMoving = true;
    }
    
    private IEnumerator StartAttack()
    {
        Player_Movement.Instance.isDisable = true;
        shakeCameraManager.ShakeCamera(0.5f);

        Vector3 retreatPosition = originalPosition + Vector3.right;
        movePoint.position = retreatPosition;
        isMoving = true;
        
        while (isMoving) yield return null;

        yield return new WaitForSeconds(0.2f);

        movePoint.position = originalPosition;
        isMoving = true;

        while (isMoving) yield return null;

        Player_Movement.Instance.isDisable = false;
        shakeCameraManager.ShakeCamera(0);
    }
    
    private IEnumerator SpawnTentacles()
    {
        Player_Movement.Instance.isDisable = true;
        shakeCameraManager.ShakeCamera(0.2f);
        
        int tentacleCount = Random.Range(tentaclesPerAttackMin, tentaclesPerAttackMax);
        HashSet<Vector2Int> usedPositions = new HashSet<Vector2Int>();

        int attempts = 0;
        int maxAttempts = 100;

        while (usedPositions.Count < tentacleCount && attempts < maxAttempts)
        {
            attempts++;

            Vector2Int offset = new Vector2Int(
                Random.Range(-tentacleActivationRadius, tentacleActivationRadius),
                Random.Range(-tentacleActivationRadius, tentacleActivationRadius)
            );

            Vector2Int cellPos = new Vector2Int(
                Mathf.RoundToInt(player.position.x) + offset.x,
                Mathf.RoundToInt(player.position.y) + offset.y
            );

            if (usedPositions.Contains(cellPos))
                continue;

            Vector3 spawnPos = new Vector3(cellPos.x + 0.5f, cellPos.y + 0.5f);
            Collider2D hitWall = Physics2D.OverlapPoint(spawnPos, wallLayer);

            if (hitWall != null)
                continue;

            usedPositions.Add(cellPos);
            
            yield return new WaitForSeconds(0.2f);
            
            Instantiate(tentacle, spawnPos, Quaternion.identity, tentacleParent);
        }

        shakeCameraManager.ShakeCamera(0);
        Player_Movement.Instance.isDisable = false;
    }

    public void Kill()
    {
       isActive = false;
    }
}
