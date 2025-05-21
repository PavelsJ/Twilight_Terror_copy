using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Interaction : MonoBehaviour
{
    private Coroutine coroutine;
    
    public float speed = 5f; 
    
    public LayerMask wallLayer;
    public LayerMask voidLayer;
    
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        PathFinding_Manager.Instance.RegisterBox(transform.position);
    }
    
    public bool TryPush(Vector3 direction)
    {
        if (gameObject.activeSelf)
        {
            Vector3 targetPosition = transform.position + direction;
        
            if (Physics2D.OverlapPoint(targetPosition, voidLayer))
            {
                Move(targetPosition);
                StartCoroutine(ToggleBox());
                return true;
            }
            else if (!Physics2D.OverlapPoint(targetPosition, wallLayer))
            {
                Move(targetPosition);
                return true;
            }
        }
        
        return false;
    }

    private void Move(Vector3 newPosition)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        
        PathFinding_Manager.Instance.SetBoxPosition(transform.position, newPosition);
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.Box);
        coroutine = StartCoroutine(MoveSmoothly(newPosition));
    }

    private IEnumerator MoveSmoothly(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition; 
    }

    private IEnumerator ToggleBox()
    {
        GetComponent<Collider2D>().enabled = false;
        
        animator.SetTrigger("FadeOut");
        
        yield return new WaitUntil(() => 
            animator.GetCurrentAnimatorStateInfo(0).IsName("FadeOut") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        
        gameObject.SetActive(false);
    }
}
