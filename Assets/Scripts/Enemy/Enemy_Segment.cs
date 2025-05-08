using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Segment : MonoBehaviour
{
    public float speed = 5f;
    public Transform movePoint;
    public bool isMoving = false;

    void Start()
    {
        movePoint.parent = PathFinding_Manager.Instance.transform;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, speed * Time.deltaTime);
            
            Vector3 direction = movePoint.position - transform.position;
            if (direction != Vector3.zero)
            {
                RotateSprite(direction); 
            }
            
            if (Vector3.Distance(transform.position, movePoint.position) < 0.05f)
            {
                isMoving = false;
            }
        }
    }

    public void MoveTo(Vector3 targetPos)
    {
        movePoint.position = targetPos;
        isMoving = true;
    }
    
    private void RotateSprite(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); 
    }
}
