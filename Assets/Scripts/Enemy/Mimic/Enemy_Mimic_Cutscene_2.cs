using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Mimic_Cutscene_2 : Enemy_Mimic_Cutscene
{
    public Transform target;
    public float speed = 8;
    public override void OnCutscene()
    {
        StartCoroutine(CutsceneCoroutine());
    }

    private IEnumerator CutsceneCoroutine()
    {
        StartCoroutine(MimicParts());
        
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

    private IEnumerator MimicParts()
    {
        agent.deactivateOnEnd = true;
        agent.EndAgent();
        
        yield return new WaitForSeconds(0.5f);
        
        foreach (var t in mimicParts)
        {
            t.SetActive(false);
        }
    }
}
