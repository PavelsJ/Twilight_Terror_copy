using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player_Cutscene : MonoBehaviour
{
    public Shake_Camera_Manager cameraManager;
    public Bed_Interaction bedInteraction;
    public Enemy_Mimic_Cutscene_4 cutscene;
    
    public GameObject beam;
    public GameObject boss;
    
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;

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
        beam.SetActive(true);
        ActivateShutter();
    }

    public void ActivateShutter()
    {
        cameraManager.ShakeCamera(1);
        originalPosition = transform.localPosition;
        StartCoroutine(ShakeCoroutine(5, 0.05f));
    }
    
    public void DeactivateShutter()
    {
        cameraManager.ShakeCamera(0);
        StopCoroutine(ShakeCoroutine(5, 0.05f));
    }

    public void ShakeSelf(float duration = 0.3f, float magnitude = 0.1f)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
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
    }

    public void EndCutscene()
    {
        bedInteraction.EndNight();
    }
}
