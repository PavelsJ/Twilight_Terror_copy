using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire_Fly : MonoBehaviour
{
    private Fire_Fly_Interation controller;
    private Vector2 targetPosition;
    private Vector2 homePosition;
    private bool escaping = false;
    private float speed;
    private float noiseOffsetX;
    private float noiseOffsetY;
    private float waveFrequency;
    private float waveAmplitude;

    public void Initialize(Fire_Fly_Interation ctrl, Vector2 home, float moveSpeed)
    {
        controller = ctrl;
        homePosition = home;
        targetPosition = controller.GetRandomPosition();
        speed = moveSpeed; 

        // Уникальные параметры для плавного движения
        noiseOffsetX = Random.Range(0f, 100f);
        noiseOffsetY = Random.Range(0f, 100f);
        waveFrequency = Random.Range(0.5f, 2f);
        waveAmplitude = Random.Range(0.2f, 0.5f);

        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        while (true)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, controller.GetPlayerPosition());

            // Если игрок рядом, убегаем
            if (!escaping && distanceToPlayer < controller.avoidDistance)
            {
                escaping = true;
                Vector2 escapeDir = (Vector2)transform.position - controller.GetPlayerPosition();
                targetPosition = (Vector2)transform.position + escapeDir.normalized * controller.avoidDistance;
            }
            // Если убегаем слишком далеко, возвращаемся
            else if (escaping && Vector2.Distance(transform.position, homePosition) > controller.avoidDistance * 1.5f)
            {
                escaping = false;
                targetPosition = controller.GetRandomPosition();
            }

            // Создаём эффект плавных колебаний (волны)
            float noiseX = Mathf.PerlinNoise(Time.time * 0.3f + noiseOffsetX, 0) - 0.5f;
            float noiseY = Mathf.PerlinNoise(Time.time * 0.3f + noiseOffsetY, 0) - 0.5f;
            float waveOffset = Mathf.Sin(Time.time * waveFrequency) * waveAmplitude;

            Vector2 finalTarget = targetPosition + new Vector2(noiseX, noiseY) + new Vector2(waveOffset, -waveOffset);

            // Плавное движение
            transform.position = Vector2.Lerp(transform.position, finalTarget, 
                (escaping ? controller.avoidSpeed : speed) * Time.deltaTime);

            yield return null;
        }
    }
    
    public IEnumerator FadeOut(float duration = 0.8f)
    {
        float time = 0f;
        GetComponent<FOD_Agent>().EndAgent();
        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            
            yield return null;
        }
    }
}