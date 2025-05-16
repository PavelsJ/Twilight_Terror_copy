using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UI_Flash : MonoBehaviour
{
    public int animationsCount = 2;
    public float flashDuration = 1;
    
    public float minTimeBetweenFlashes = 2;
    public float maxTimeBetweenFlashes = 4;
    
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        
        if (animator != null)
        {
            StartCoroutine(FlashRoutine());
        }
    }
    
    private IEnumerator FlashRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minTimeBetweenFlashes, maxTimeBetweenFlashes);
            yield return new WaitForSeconds(waitTime);

            int randomIndex = Random.Range(0, animationsCount);
            string triggerName = "Flash" + randomIndex;

            animator.SetTrigger(triggerName);

            yield return new WaitForSeconds(flashDuration);
        }
    }
}
