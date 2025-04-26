using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cracked_Path_Trap : MonoBehaviour
{
    public RuleTile voidTile;
    private Tilemap tilemap;   
    
    private Vector3Int tilePosition;
    
    public bool isActive = false;
    private Collider2D col;
    
    private void Start()
    {
        if (tilemap == null)
        {
            tilemap = GetComponentInParent<Tilemap>(); 
        }
        
        tilePosition = tilemap.WorldToCell(transform.position);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!isActive && (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Box")))
        {
            SetTrap();
        }
    }

    private void SetTrap()
    {
        isActive = true;
        
        if (tilemap != null && voidTile != null)
        {
            tilemap.SetTile(tilePosition, voidTile);
        }
        
        col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
    }
}
