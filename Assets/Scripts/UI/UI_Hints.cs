using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Hints : MonoBehaviour
{
    public Image hint;
    public TextMeshProUGUI hintText;
    
    private Animator animator;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void ShowHint(Sprite newSprite, string newText)
    {
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.Hint);
        
        gameObject.SetActive(true);
        hint.sprite = newSprite;
        hintText.text = newText;
        
        animator.SetTrigger("ShowHint");
    }

    public void HideHint()
    {
        if (gameObject.activeInHierarchy)
        {
            Music_Manager.instance.PlaySound(Music_Manager.SoundType.Hint);
            StartCoroutine(HideHintCoroutine());
        }
    }

    private IEnumerator HideHintCoroutine()
    {
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        
        animator.SetTrigger("HideHint");
        
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            
        hint.sprite = null;
        gameObject.SetActive(false);
    }
}
