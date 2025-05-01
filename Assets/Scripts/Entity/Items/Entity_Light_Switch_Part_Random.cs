using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Light_Switch_Part_Random : Entity_Light_Switch_Part, ISwitchPart
{
    [Header("Boss Fight Settings")] 
    public Light_Switch_Positions_Manager manager;
    private Transform newTarget;

    public void RandomizePosition()
    {
        newTarget = manager.RandomizePosition(parentPos);
        FadeOut();
    }

    protected override void RemoveSwitch()
    {
        Enemy_Mimic_Movement mimic = FindObjectOfType<Enemy_Mimic_Movement>(true);
        
        if (mimic != null)
        {
            mimic.RemoveSwitchPart(parentPos.gameObject);
            mimic.isSwapping = false;
        }
    }
    
    private void FadeOut()
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(MoveToTarget());
    }
    
    private IEnumerator MoveToTarget()
    {
        isMoving = true;
        parentPos.position = newTarget.position;
        
        Vector3 startPos = transform.position;
        Vector3 jumpPeak = startPos + new Vector3(0f, 0.5f, 0f);
    
        float jumpDuration = 0.3f;
        float jumpElapsed = 0f;

        while (jumpElapsed < jumpDuration)
        {
            jumpElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(jumpElapsed / jumpDuration);
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPos, jumpPeak, easedT);
            yield return null;
        }
        
        Vector3 peakPos = transform.position;
        float distance = Vector3.Distance(peakPos, newTarget.position);
        float moveDuration = distance / speed;

        float moveElapsed = 0f;
        while (moveElapsed < moveDuration)
        {
            moveElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(moveElapsed / moveDuration);
            float easedT = 1f - Mathf.Pow(1f - t, 3); // ease-out
            transform.position = Vector3.Lerp(peakPos, newTarget.position, easedT);
            yield return null;
        }

        transform.position = parentPos.position;
        transform.localPosition = parentPos.position;
        startY = parentPos.position.y;
        
        GetComponent<Collider2D>().enabled = true;
        isMoving = false;
    }
}
