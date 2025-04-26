using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Static_Enemy_Lurking_Shadow : MonoBehaviour
{
    [Header("Shadow Settings")]
    public float chaseDistance = 3.5f;
    public bool isNear = false;
    
    [Header("Transform References")] 
    public Transform player;
    
    [Header("Compounds")]
    public SpriteMask spriteMask;
    private FOD_Agent fodAgent; 
    
    void Start()
    {
        
        fodAgent = player.GetComponent<FOD_Agent>();
        spriteMask.enabled = false;
    }
    
    private void Update()
    {
        CheckDistanceToPlayer();
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
}
