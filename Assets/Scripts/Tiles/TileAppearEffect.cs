using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapAppearEffect : MonoBehaviour
{
    public float moveDuration = 1f;

    private Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    private void OnEnable()
    {
        StartCoroutine(AppearEffect());
    }

    private IEnumerator AppearEffect()
    {
        tilemap.color = new Color(1, 1, 1, 0); 

        float elapsedTime = 0;
        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            tilemap.color = new Color(1, 1, 1, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        tilemap.color = new Color(1, 1, 1, 1);
    }
}