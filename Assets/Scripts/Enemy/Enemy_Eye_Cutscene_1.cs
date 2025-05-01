using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Eye_Cutscene_1 : MonoBehaviour
{
    public Transform eyePupil;
    public Transform eyeballCenter; 
    
    public float maxOffset = 0.1f;    
    private Transform playerTarget;
    void OnEnable()
    {
        playerTarget = Player_Movement.Instance.transform;

        StartCoroutine(OnCutscene());
    }

    private IEnumerator OnCutscene()
    {
        if (playerTarget == null) yield break;

        float duration = 2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            
            Vector3 direction = playerTarget.position - eyeballCenter.position;
            direction.z = 0f; 
            
            Vector3 offset = Vector3.ClampMagnitude(direction.normalized * maxOffset, maxOffset);
            eyePupil.localPosition = offset;

            yield return null;
        }
    }
}
