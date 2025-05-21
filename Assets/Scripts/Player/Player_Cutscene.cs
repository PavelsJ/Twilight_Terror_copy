using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player_Cutscene : MonoBehaviour
{
    public Shake_Camera_Manager cameraManager;
    public Enemy_Mimic_Cutscene_4 cutscene;
    
    public GameObject beam;
    public Collider2D blockCollider;
    
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
       
    }

    public void ActivateFlashLight()
    {
        spriteRenderer.flipX = true;
        animator.SetTrigger("ActivateFlashLight");
    }
    
    public void ActivateBeam()
    {
        Music_Manager.instance.PlayOneSound(Music_Manager.SoundType.Blaze);
        beam.SetActive(true);
        
        cameraManager.ShakeCamera(1);
        originalPosition = transform.localPosition;
        shakeCoroutine = StartCoroutine(ShakeCoroutine(5, 0.05f));
    }
    
    public void DeactivateBeam()
    {
        beam.SetActive(false);
        animator.SetTrigger("DeactivateFlashLight");
        blockCollider.enabled = false;
        
        StopCoroutine(shakeCoroutine);
        shakeCoroutine = null;
        
        cameraManager.ShakeCamera(0);
        cutscene.DeactivateMimic();
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    public void KillMimic()
    {
        cutscene.KillMimic();
        Music_Manager.instance.PlayOneSound(Music_Manager.SoundType.Blaze,1);
    }

    
}
