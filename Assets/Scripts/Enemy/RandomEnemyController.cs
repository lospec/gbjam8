using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Enemy;

namespace Enemy
{
    public class RandomEnemyController : EnemyController
    {
        [Header("Random Movement Properties")]
        public Vector2 movementArea;
        public float minChangeDirectionDistance = 0.2f;
        public float speed = 5;
        public float stillTime = 2f;

        private Vector3 currentDestination;
        private Vector3 startPosition;
        private bool canChangeDirection;

        override protected void Start()
        {
            canChangeDirection = true;
            startPosition = transform.position;
            currentDestination = Vector3.zero;
            base.Start();
        }
        // Update is called once per frame
        void Update()
        {
            if ((currentDestination.Equals(Vector3.zero) || Vector2.Distance(transform.position, currentDestination) < minChangeDirectionDistance) 
                && canChangeDirection)
            {
                Vector3 offset = new Vector3(
                    Random.Range(-movementArea.x, movementArea.x), Random.Range(-movementArea.y, movementArea.y), 0
                );
                currentDestination = startPosition + offset;

                StartCoroutine(ResetChangeDirection());
            }

            body.velocity = (currentDestination - transform.position) * speed;
        }

        private IEnumerator ResetChangeDirection()
        {
            canChangeDirection = false;

            yield return new WaitForSeconds(stillTime);

            canChangeDirection = true;
        }
    }
}

