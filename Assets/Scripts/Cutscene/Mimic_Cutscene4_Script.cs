using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic_Cutscene4_Script : MonoBehaviour
{
    public Transform midPoint;

    public Transform playerMovePoint;
    public Transform playerTransform;

    public Transform enemyMovePoint;
    public Transform enemyTransform;
    
    public Enemy_Mimic_Movement mimicMovement;
    private bool isActive;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!isActive && mimicMovement != null)
            {
                mimicMovement.ActivateMimic();
                isActive = true;
            }
        }
    }
}
