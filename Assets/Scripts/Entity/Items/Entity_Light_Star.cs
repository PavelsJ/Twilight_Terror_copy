using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Light_Star : MonoBehaviour, IInteractable
{
    public GameObject UIprefab;
    private bool isMoving = false;
    
    [Header("Motion Settings")]
    public float amplitude = 0.1f;
    public float cycleDuration = 4f; 

    private float startY;
    private float timer;
    
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("FadeIn");
        
        startY = transform.position.y;
        timer = UnityEngine.Random.Range(0f, cycleDuration); 
    }
    
    void Update()
    {
        if (!isMoving)
        {
            timer += Time.deltaTime;
            float t = (Mathf.Sin((timer / cycleDuration) * 2 * Mathf.PI) + 1f) / 2f; 
            float yOffset = Mathf.SmoothStep(-amplitude, amplitude, t);
        
            Vector3 pos = transform.position;
            pos.y = startY + yOffset;
            transform.position = pos;
        }
    }

    public void DestroyObject()
    {
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.PickUp);
        UI_Inventory.Instance.AddItem(UIprefab);
        FadeOut();
    }

    private void FadeOut()
    {
        GetComponent<Collider2D>().enabled = false;
        anim.SetTrigger("FadeOut");

        StartCoroutine(MoveToTarget());
        
        FOD_Agent agent = gameObject.GetComponent<FOD_Agent>();
        agent.deactivateOnEnd = true;
        agent.EndAgent();
    }
    
    private IEnumerator MoveToTarget()
    {
        isMoving = true;
    
        Vector3 startPos = transform.position;
        Vector3 jumpPeak = startPos + new Vector3(0f, 0.5f, 0f);
        Vector3 endPos = startPos + new Vector3(0f, -0.5f, 0f); 

        float jumpDuration = 0.3f;
        float fallDuration = 0.4f;

        // Up
        float elapsed = 0f;
        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / jumpDuration);
            float easedT = Mathf.Sin(t * Mathf.PI * 0.5f); // easeOutSine
            transform.position = Vector3.Lerp(startPos, jumpPeak, easedT);
            yield return null;
        }

        // Down
        elapsed = 0f;
        while (elapsed < fallDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fallDuration);
            float easedT = 1f - Mathf.Cos(t * Mathf.PI * 0.5f); // easeInSine
            transform.position = Vector3.Lerp(jumpPeak, endPos, easedT);
            yield return null;
        }
    }
}
