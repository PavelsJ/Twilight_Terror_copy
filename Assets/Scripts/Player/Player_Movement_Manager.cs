using System.Collections;
using System.Collections.Generic;
using FODMapping;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Movement_Manager : MonoBehaviour
{
    public static Player_Movement_Manager Instance { get; private set; }
    
    [Header("Transforms")]
    public Transform player;
    public Transform enemy;
    public GameObject[] obstaclesOnPath;
    
    private Vector3 lastMoveDirection = Vector3.zero;
    
    [Header("Stats")] 
    public bool isInvulnerable;
    public bool isStealth;
    
    [Header("Compounds")]
    private Player_Stats stats;
    private FOD_Manager manager;
    private List<IEnemy> enemies = new List<IEnemy>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (player == null)
        {
            player = Player_Movement.Instance.transform;
        }
        
        stats = GetComponent<Player_Stats>();
        manager = FindObjectOfType<FOD_Manager>(true);
        
        Save_Manager.Instance.SaveScene();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            NotifyEnemiesOfPlayerMove();
        }
        
        if (Input.GetKeyDown(KeyCode.G))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            ActivateFog();
        }
    }

    public void RegisterEnemy(IEnemy enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }
    
    public void DeregisterEnemy(IEnemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
    }
    
    public void NotifyEnemiesOfPlayerMove()
    {
        if (!isStealth)
        {
            stats.UpdateMoveCount();
        
            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                if (enemy is MonoBehaviour { gameObject: { activeInHierarchy: true } })
                {
                    enemy.OnPlayerMoved();
                }
            }
        }
    }
    
    public void AddSteps(int amount)
    {
       stats.AddSteps(amount);
    }
    
    public void AddLife()
    {
        stats.AddLife();
    }

    public void ActivateCentipedeChase()
    {
        Music_Manager.instance.SetToChaseMusic();
        Music_Manager.instance.PlayOneSound(Music_Manager.SoundType.Warning,1);
        
        if(enemy != null) enemy.gameObject.SetActive(true);

        ClearPath();
    }

    private void ClearPath()
    {
        if (obstaclesOnPath.Length > 0)
        {
            foreach (var obstacle in obstaclesOnPath)
            {
                obstacle.SetActive(false);
            }
        }
    }

    public void ActivateSpiderChase()
    {
        if (enemies.Count > 0)
        {
            Music_Manager.instance.SetToChaseMusic();
            Music_Manager.instance.PlayOneSound(Music_Manager.SoundType.Warning);
            
            foreach (var enemy in enemies)
            {
                if (enemy is Enemy_Spider_Chase spiderMovement)
                {
                    spiderMovement.isChasingPlayer = true;
                    spiderMovement.spriteMask.enabled = true;
                }
            }
        }
    }

    private void ActivateFog()
    {
        if (manager != null)
        {
            manager.gameObject.SetActive(true);
            manager.StartCoroutine(manager.EnableWithDelay(0.8f));
        }
    }
    
    public void SetPlayerMoveDirection(Vector3 direction)
    {
        lastMoveDirection = direction;
    }

    public Vector3 GetLastMoveDirection()
    {
        return lastMoveDirection;
    }
    
    public void SetInvulnerability(bool state)
    {
        isInvulnerable = state;

        if (isInvulnerable)
        {
            stats.SetMaxSteps();
        }
    }

    public void SetNewStats()
    {
        stats.DescreasePlayerLives();
    }

    public void RestartGame()
    {
        StartCoroutine(RestartCoroutine());
    }
    
    private IEnumerator RestartCoroutine()
    {
        if (manager != null)
        {
            manager.RemoveAgentsGradually();
        }

        yield return new WaitForSeconds(0.5f);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
