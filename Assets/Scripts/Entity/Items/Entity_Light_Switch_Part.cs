using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Light_Switch_Part : Entity_Motion, IInteractable
{
    public Transform target;
    public float speed = 8;

    internal Transform parentPos;
   
    private Light_Switch_Cutscene lightSwitch;
    
    void Start()
    {
        parentPos = transform.parent;
        transform.parent = Player_Movement_Manager.Instance.transform;

        if (target != null)
        {
            lightSwitch = target.GetComponent<Light_Switch_Cutscene>();
        }
    }
    
    public void DestroyObject()
    {
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.PickUp);
        RemoveSwitch();
        FadeOut();
    }
    
    protected virtual void RemoveSwitch()
    {
        
    }
    
    private void FadeOut()
    {
        GetComponent<Collider2D>().enabled = false;

        if (target != null)
        {
            StartCoroutine(MoveToTarget());
        }
        
        FOD_Agent agent = gameObject.GetComponent<FOD_Agent>();
        agent.deactivateOnEnd = true;
        agent.EndAgent();
        
        lightSwitch.AddPart(1);
    }
    
    private IEnumerator MoveToTarget()
    {
        isMoving = true;
        
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
        float distance = Vector3.Distance(peakPos, target.position);
        float moveDuration = distance / speed;

        float moveElapsed = 0f;
        while (moveElapsed < moveDuration)
        {
            moveElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(moveElapsed / moveDuration);
            float easedT = 1f - Mathf.Pow(1f - t, 3); // ease-out
            transform.position = Vector3.Lerp(peakPos, target.position, easedT);
            yield return null;
        }

        transform.position = target.position;
    }
}
