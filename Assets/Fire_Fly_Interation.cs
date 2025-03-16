using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FODMapping
{
    public class Fire_Fly_Interation : MonoBehaviour
    {
        public float maxRange = 1f;
        public float speed = 1f;
        public float sightRange = 1f;
        public float fleeDistance = 2f;
        private Vector3 origin;
        private bool fleeing;
        private Transform player;

        private void Start()
        {
            origin = transform.position;
            StartCoroutine(MoveRandomly());
        }

        private void Update()
        {
            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.position);
                if (distanceToPlayer < fleeDistance && !fleeing)
                {
                    fleeing = true;
                    StartCoroutine(Flee());
                }
                else if (distanceToPlayer > fleeDistance && fleeing)
                {
                    fleeing = false;
                    StartCoroutine(ReturnToOrigin());
                }
            }
        }

        public void SetPlayer(Transform playerTransform)
        {
            player = playerTransform;
        }

        private IEnumerator MoveRandomly()
        {
            while (true)
            {
                if (!fleeing)
                {
                    Vector3 randomDirection = Random.insideUnitCircle * maxRange;
                    Vector3 targetPos = origin + new Vector3(randomDirection.x, randomDirection.y, 0f);
                    float journey = 0f;
                    float duration = Random.Range(0.5f, 1.5f);
                    Vector3 startPos = transform.position;
                    while (journey < duration)
                    {
                        transform.position = Vector3.Lerp(startPos, targetPos, journey / duration);
                        journey += Time.deltaTime * speed;
                        yield return null;
                    }
                }
                yield return new WaitForSeconds(Random.Range(0.5f, 1f));
            }
        }

        private IEnumerator Flee()
        {
            Vector3 direction = (transform.position - player.position).normalized * fleeDistance;
            Vector3 targetPos = transform.position + direction;
            float journey = 0f;
            while (journey < 1f)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, journey);
                journey += Time.deltaTime * speed;
                yield return null;
            }
        }

        private IEnumerator ReturnToOrigin()
        {
            float journey = 0f;
            Vector3 startPos = transform.position;
            while (journey < 1f)
            {
                transform.position = Vector3.Lerp(startPos, origin, journey);
                journey += Time.deltaTime * speed;
                yield return null;
            }
        }
    }
}
