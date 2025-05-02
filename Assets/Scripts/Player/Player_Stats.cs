using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Stats : MonoBehaviour
{
    [Header("Player Stats")] 
    public int stepCount = 20;
    public int playerLives = 2;
    private readonly int radiusShrinkValue = 12;
   
    [Header("UI Settings")]
    public Image healthSlot;
    public Sprite[] healthSprites;
    public TextMeshProUGUI stepCountText;
    
    private int maxStepCount;
    private Coroutine stepCountAnimationCoroutine;
    private FOD_Agent agent;
    
    void Start()
    {
        maxStepCount = stepCount;
        agent = FindObjectOfType<Player_Movement>(true).GetComponent<FOD_Agent>();
        
        UpdateStepCountText();
    }
    
    public void AddSteps(int steps)
    {
        stepCount += steps;
        UpdateStepCountText();
    }

    public void SetMaxSteps()
    {
        if (stepCount >= maxStepCount) return;
        
        stepCount = maxStepCount;
        UpdateStepCountText();
    }
    
    public void AddLife()
    {
        playerLives++;
        
       
        int oldMax = maxStepCount;
        maxStepCount *= 2;

        float radius = agent.GetRadius();
        agent.ChangeRadiusValue(radius + radiusShrinkValue);

        StartStepUpdate(oldMax, maxStepCount);
        UpdateStepCountText();
    }

    public void UpdateMoveCount()
    {
        IncrementMoveCount();
    }
    
    private void IncrementMoveCount()
    {
        if (Player_Movement_Manager.Instance.isInvulnerable || playerLives == 0) return;
        
        stepCount--;
        
        if (stepCount <= 1 && !UI_Inventory.Instance.IsInventoryEmpty())
        {
            UI_Inventory.Instance.RemoveItem();
        }
        
        if (stepCount <= 0 )
        {
            playerLives--;
            healthSlot.sprite = healthSprites[playerLives];
           
            float radius = agent.GetRadius();
            agent.ChangeRadiusValue(radius - radiusShrinkValue);
            
            if (playerLives <= 0)
            {
                Player_Movement_Manager.Instance.ActivateCentipedeChase();
                healthSlot.sprite = healthSprites[0];
                int oldMax = maxStepCount;
                maxStepCount = stepCount;
                StartStepUpdate(oldMax, maxStepCount);
                return;
            }
            
            stepCount += maxStepCount / 2;
            int oldMax2 = maxStepCount;
            maxStepCount = stepCount;
            
            StartStepUpdate(oldMax2, maxStepCount);
        }
        
        UpdateStepCountText();
    }

    public void DescreasePlayerLives()
    {
        int oldMax = maxStepCount;

        if (maxStepCount >= 10)
        {
            stepCount = maxStepCount / 2;
            maxStepCount = stepCount;
            
            StartStepUpdate(oldMax, maxStepCount);
        }

        if (playerLives > 1)
        {
            playerLives--;

            float radius = agent.GetRadius();
            agent.ChangeRadiusValue(radius - radiusShrinkValue);
            healthSlot.sprite = healthSprites[playerLives];
        }

        UpdateStepCountText();
    }
    
    private void StartStepUpdate(int from, int to)
    {
        if (stepCountAnimationCoroutine != null)
            StopCoroutine(stepCountAnimationCoroutine);
        stepCountAnimationCoroutine = StartCoroutine(AnimateStepChange(from, to));
    }

    private IEnumerator AnimateStepChange(int from, int to)
    {
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            int currentMax = Mathf.RoundToInt(Mathf.Lerp(from, to, elapsed / duration));

            if (stepCountText != null)
                stepCountText.text = $"Hope - {stepCount:00}, ({currentMax:00})";

            yield return null;
        }

        if (stepCountText != null)
            stepCountText.text = $"Hope - {stepCount:00}, ({to:00})";
        
        stepCountAnimationCoroutine = null;
    }
    
    private void UpdateStepCountText()
    {
        if (stepCountAnimationCoroutine == null && stepCountText != null)
        {
            stepCountText.text = $"Hope - {stepCount:00}, ({maxStepCount:00})";  
        }
    }
}
