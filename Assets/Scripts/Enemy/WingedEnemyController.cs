using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Enemy
{
    public class WingedEnemyController : EnemyController
    {
        public enum StartState
        {
            Grounded,
            Flying
        }

        private static class AnimParams
        {
            public static readonly int IsFlying = Animator.StringToHash("IsFlying");
        }

        public float searchRange = 10f;
        public float moveSpeed = 15f;
        public float takeOffHeight = 3f;

        [SerializeField] private LayerMask groundLayer;

        [SerializeField] private CircleCollider2D flyingCollider;
        [SerializeField] private BoxCollider2D groundCollider;

        protected override void Start()
        {
            base.Start();
            body.gravityScale = 0;
            enabled = false;
        }

        public void Initialize(Transform player, StartState state)
        {
            Initialize(player);
            bool StartCondition() => DistanceToPlayer <= searchRange && !enabled;
            switch (state)
            {
                case StartState.Grounded:
                    StartCoroutine(Drop());
                    InvokeWhen(() => StartCoroutine(TakeOff()), StartCondition, 2f);
                    break;
                case StartState.Flying:
                    flyingCollider.enabled = true;
                    animator.SetBool(AnimParams.IsFlying, true);
                    InvokeWhen(() => enabled = true, StartCondition, 2f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }


        private void Update()
        {
            if (velocity.x != 0)
            {
                spriteRenderer.flipX = !(velocity.x > 0);
            }

            if (DistanceToPlayer <= searchRange)
            {
                InvokeWhen(() => { StartCoroutine(IncreaseAltitude(takeOffHeight)); },
                    () => DistanceToPlayer >= searchRange * 2f && enabled, 2f);
            }
        }

        private void FixedUpdate()
        {
            var move = DirectionToPlayer;
            velocity = move * moveSpeed;
            body.AddForce(velocity);
        }

        private IEnumerator IncreaseAltitude(float height)
        {
            var startPos = transform.position;
            var waitForFixedUpdate = new WaitForFixedUpdate();
            while (transform.position.y - startPos.y <= height)
            {
                body.AddForce(Vector2.up * moveSpeed);
                yield return waitForFixedUpdate;
            }
        }

        private IEnumerator TakeOff()
        {
            body.gravityScale = 0f;
            animator.SetBool(AnimParams.IsFlying, true);
            flyingCollider.enabled = true;
            groundCollider.enabled = false;
            yield return StartCoroutine(IncreaseAltitude(takeOffHeight));
            body.velocity = Vector2.zero;
            enabled = true;
        }

        private IEnumerator Drop()
        {
            flyingCollider.enabled = false;
            groundCollider.enabled = true;
            animator.SetBool(AnimParams.IsFlying, false);
            body.gravityScale = 10f;
            var result = new Collider2D[1];
            var bounds = groundCollider.bounds;
            Physics2D.OverlapAreaNonAlloc(bounds.min, bounds.max, result, groundLayer);
            while (!result.First())
            {
                yield return null;
            }

            body.velocity = Vector2.zero;
        }
    }
}