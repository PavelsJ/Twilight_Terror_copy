using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Fire_Fly_Interation : MonoBehaviour
{
    [Header("Firefly Count")]
    public int fireflyCountMin = 5;  
    public int fireflyCountMax = 10;  

    [Header("Flight Area")]
    public Vector2 areaSize = new Vector2(5f, 5f);  

    [Header("Speed Settings")]
    public float speedMin = 0.2f;  
    public float speedMax = 0.5f;  

    [Header("Player Avoidance")]
    public float avoidSpeed = 4f;  
    public float avoidDistance = 2f;  

    [Header("References")]
    public GameObject fireflyPrefab;  
    public Transform player;  

    private List<Fire_Fly> fireflies = new List<Fire_Fly>();

    void Start()
    {
        int fireflyCount = Random.Range(fireflyCountMin, fireflyCountMax + 1); // Случайное количество
        for (int i = 0; i < fireflyCount; i++)
        {
            Vector2 randomPos = GetRandomPosition();

            GameObject newFirefly = Instantiate(fireflyPrefab, randomPos, Quaternion.identity, transform);
            Fire_Fly fireflyScript = newFirefly.AddComponent<Fire_Fly>();

            float randomSpeed = Random.Range(speedMin, speedMax);
            fireflyScript.Initialize(this, randomPos, randomSpeed);
            
            fireflies.Add(fireflyScript);
        }
    }

    public Vector2 GetRandomPosition()
    {
        return (Vector2)transform.position + new Vector2(
            Random.Range(-areaSize.x / 2, areaSize.x / 2),
            Random.Range(-areaSize.y / 2, areaSize.y / 2)
        );
    }

    public Vector2 GetPlayerPosition()
    {
        return player.position;
    }

    public void Deactivate()
    {
        StartCoroutine(FadeOutFireflies());
    }

    private IEnumerator FadeOutFireflies()
    {
        for (int i = fireflies.Count - 1; i >= 0; i--)
        {
            yield return fireflies[i].FadeOut(); 
            fireflies[i].gameObject.SetActive(false);
            fireflies.RemoveAt(i);
        }
    }
}