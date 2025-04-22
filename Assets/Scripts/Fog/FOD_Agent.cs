using System;
using System.Collections;
using System.Collections.Generic;
using FODMapping;
using UnityEngine;

public class FOD_Agent : MonoBehaviour
{
    [Header("Agent State")]
    public bool isActive = false;
    public bool deactivateOnEnd = false;
    
    [Header("Agent Customization")]
    [Range(0.0f, 480.0f)] public float sightRange = 50.0f;
    [Range(0.1f, 10)] public float sightRangeDifference = 2f;
    [Range(0.0f, 1.0f)] public float sightTransparency = 1f;

    [Header("Light_Flickering")]
    public bool flickering = true;
    public float flickeringSpeed = 4;
    private float curFlickeringSpeed = 4;

    private float baseRadius = 0;
    private float targetRadius;
    private float currentRadius;

    private bool increasing = false;
    
    private FOD_Manager manager;
    private Coroutine updateRoutine;
    private Action fogInitCallback;

    private void Awake()
    {
        manager = FindObjectOfType<FOD_Manager>(true);

        curFlickeringSpeed = flickeringSpeed;
        currentRadius = sightRange;
        baseRadius = sightRange;
        targetRadius = baseRadius - sightRangeDifference;
    }

    private void OnEnable()
    {
        if (manager == null) return;
        
        isActive = true;
        manager.AddAgent(this);
        
        if (manager.IsFogInitialized)
        {
            StartAgent();
        }
        else
        {
            fogInitCallback = () => StartAgent();
            manager.OnFogInitialized += fogInitCallback;
        }
    }
    
    private void OnDisable()
    {
        if (manager != null)
        {
            if (fogInitCallback != null)
            {
                manager.OnFogInitialized -= fogInitCallback;
            }

            if (!isActive)
            {
                manager.RemoveAgent(this);
            }
        }
    }
    
    private void OnBecameVisible()
    {
        if (!isActive && !gameObject.CompareTag("Player"))
        {
            ActivateAgent();
        }
    }
    
    private void OnBecameInvisible()
    {
        if (isActive && !gameObject.CompareTag("Player"))
        {
            DeactivateAgent();
        }
    }
    
    public void ActivateAgent()
    {
        if (isActive) return;
        
        isActive = true;
        manager.AddAgent(this);
        StartAgent();
    }
    
    private void DeactivateAgent()
    {
        if (!isActive) return;
        
        EndAgent();
    }
    
    private void StartAgent(float duration = 1f)
    {
        StopFlickering();
        updateRoutine = StartCoroutine(ChangeRadiusSmoothly(0, currentRadius, duration));
    }

    public void EndAgent(float duration = 0.7f)
    {
        StopFlickering();
        
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(FadeOut(duration));
        }
    }
    
    private IEnumerator FadeOut(float duration)
    {
        yield return ChangeRadiusSmoothly(sightRange, 0, duration);

        if (!gameObject.CompareTag("Player"))
        {
            isActive = false;
            manager.RemoveAgent(this);
            
            if (deactivateOnEnd)
            {
                yield return new WaitForSeconds(0.2f); 
                gameObject.SetActive(false);
            }
        }
    }
    
    private void StartFlickering()
    {
        if (updateRoutine == null)
        {
            updateRoutine = StartCoroutine(UpdateAgent());
        }
    }

    private void StopFlickering()
    {
        if (updateRoutine != null)
        {
            StopCoroutine(updateRoutine);
            updateRoutine = null;
        }
    }

    private IEnumerator UpdateAgent()
    {
        float updateInterval = 1f / 60f; 
        
        while (true)
        {
            yield return new WaitForSecondsRealtime(updateInterval);
            UpdateRadiusValues();
        }
    }

    private void UpdateRadiusValues()
    {
        sightRange = Mathf.MoveTowards(sightRange, targetRadius, flickeringSpeed * Time.unscaledDeltaTime);

        if (Mathf.Abs(sightRange - targetRadius) < 0.1f)
        {
            increasing = !increasing;
            targetRadius = increasing ? baseRadius : baseRadius - sightRangeDifference;
        }
    }
    
    public void ChangeRadiusValue(float newRadius)
    {
        currentRadius = newRadius;
        
        StopFlickering();
        updateRoutine = StartCoroutine(ChangeRadiusSmoothly(sightRange, currentRadius));
        
        curFlickeringSpeed += 6;
        flickeringSpeed = curFlickeringSpeed;
    }
    
    public void SetMinRadiusValue()
    {
        StopFlickering(); 
        updateRoutine = StartCoroutine(ChangeRadiusSmoothly(sightRange, 22));
        
        flickeringSpeed = 16;
    }

    public void SetMaxRadiusValue()
    {
        StopFlickering();
        updateRoutine = StartCoroutine(ChangeRadiusSmoothly(sightRange, currentRadius));
        
        flickeringSpeed = curFlickeringSpeed;
    }

    private IEnumerator ChangeRadiusSmoothly(float start, float end, float duration = 1)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            sightRange = Mathf.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        
        sightRange = end;
        baseRadius = end;
        targetRadius = baseRadius - sightRangeDifference;

        updateRoutine = null;
        
        StartFlickering();
    }

    public float GetRadius()
    {
        return currentRadius;
    }
}
