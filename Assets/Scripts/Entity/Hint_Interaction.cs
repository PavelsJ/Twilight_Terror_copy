using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hint_Interaction : Entity_Motion
{
    [Header("Hint Settings")]
    public Sprite hint;
    public string hintText;
    public UI_Hints hints;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isMoving)
        {
            isMoving = true;
            GetComponent<SpriteRenderer>().color = Color.gray;
            hints.ShowHint(hint, hintText);
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && isMoving)
        {
            hints.HideHint();
        }
    }
}


