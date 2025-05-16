using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprite_Mask_Animation : MonoBehaviour
{
    public Sprite[] sprites;
    public SpriteMask mask;
    private void Awake()
    {
        if (sprites != null && sprites.Length > 0 && mask != null)
        {
            mask.sprite = sprites[0];
        }
    }

    public void ChangeSprite(int spriteIndex)
    {
        if (mask == null)
        {
            mask = GetComponentInChildren<SpriteMask>();
        }
        
        mask.sprite = sprites[spriteIndex];
    }

    public void ChangeSprite0()
    {
        mask.sprite = sprites[0];
    }
    
    public void ChangeSprite1()
    {
        mask.sprite = sprites[1];
    }
    
    public void ChangeSprite3()
    {
        mask.sprite = sprites[1];
    }
}
