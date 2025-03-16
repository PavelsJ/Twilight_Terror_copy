using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Stats : MonoBehaviour
{
    [Header("Player Stats")] 
    public int stepCount = 20;
    public int playerLives = 3;
   
    [Header("UI Settings")]
    public Image healthSlot;
    
    public Sprite damagedHealthSlot;
    public Sprite zeroHealthSlot;
    
    private int maxStepCount;
    public TextMeshProUGUI stepCountText;
    
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

    public void UpdateMoveCount()
    {
        IncrementMoveCount();
    }
    
    private void IncrementMoveCount()
    {
        if (Player_Movement_Manager.Instance.isInvulnerable || playerLives == 0) return;
        stepCount--;  
        
        if (stepCount <= 1)
        {
            if (!UI_Inventory.Instance.IsInventoryEmpty())
            {
                UI_Inventory.Instance.RemoveItem();
            }
        }
        
        if (stepCount <= 0 )
        {
            playerLives--;
            
            healthSlot.sprite = damagedHealthSlot;
            
           
            float radius = agent.sightRange;
            agent.ChangeRadiusValue(radius - 8);
            
            if (playerLives <= 0)
            {
                Player_Movement_Manager.Instance.ActivateCentipedeChase();
                healthSlot.sprite = zeroHealthSlot;
                maxStepCount = stepCount;
                UpdateStepCountText();
                return;
            }
            
            stepCount += maxStepCount / 2;
            maxStepCount = stepCount;
        }
        
        UpdateStepCountText();
    }
    
    private void UpdateStepCountText()
    {
        if (stepCountText != null)
        {
            stepCountText.text = $"Hope - {stepCount:00}, ({maxStepCount:00})";  
        }
    }
}
