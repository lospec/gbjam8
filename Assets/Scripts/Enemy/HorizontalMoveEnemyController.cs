using UnityEngine;

namespace Enemy
{
    public class HorizontalMoveEnemyController : EnemyController
    {
        [Header("HorizontalMover Properties")]
        public float leftOffset;
        public float rightOffset;
        public float speed;
        public float changeDirectionDistance;

        private float endX;
        private float startX;
        private Vector3 velocityVector;
        private bool isGoingRight;
        // Start is called before the first frame update
        void Start()
        {
            endX = transform.position.x + rightOffset;
            startX = transform.position.x - leftOffset;
        
            isGoingRight = Random.Range(0, 10) > 5 ? true : false;
            velocityVector = new Vector3(isGoingRight ? speed : -speed, 0, 0);

            base.Start();
        }

        // Update is called once per frame
        void Update()
        {
            if ((transform.position.x + changeDirectionDistance >= endX && isGoingRight) || 
                (transform.position.x - changeDirectionDistance <= startX && !isGoingRight))
            {
                velocityVector *= -1;
                spriteRenderer.flipX = !spriteRenderer.flipX;
                isGoingRight = !isGoingRight;
            }

            body.velocity = velocityVector;
        }
    }
}
