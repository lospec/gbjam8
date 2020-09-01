using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMotor : MonoBehaviour
    {
        private const float MoveSpeedModifier = 10f;
        public LayerMask floorLayer;
        public float moveSpeed = 10f;
        public float jumpPower = 30f;
        public float jumpTimer = 0.1f;
        public float customGravity = 9.88f;
        public float terminalVelocity = 50f;

        [SerializeField] private Transform topLeftBound = default;
        [SerializeField] private Transform bottomRightBound = default;

        private float _jumpTime;
        private bool _isGrounded;
        private Vector2 _gravity;
        private Collider2D[] _groundCollision = new Collider2D[1];

        public Rigidbody2D Body { get; private set; }
        public bool IsJumping { get; private set; }
        public Vector2 Move { get; set; }

        private void Awake()
        {
            Body = GetComponent<Rigidbody2D>();
            Body.gravityScale = 0;
        }

        private void FixedUpdate()
        {
            _isGrounded = GroundCheck();

            _gravity = _isGrounded
                ? Vector2.zero
                : _gravity + Vector2.down * customGravity;

            Body.AddForce(_gravity);

            var velocity = Body.velocity;
            velocity.x = Mathf.Abs(Move.x) > 0
                ? Mathf.Sign(Move.x) * moveSpeed
                : velocity.x;

            velocity.y = velocity.y < -terminalVelocity
                ? -terminalVelocity
                : velocity.y;

            Body.velocity = velocity;
            Move = Vector2.zero;
        }


        public IEnumerator Jump(InputAction contextAction)
        {
            if (!_isGrounded || IsJumping) yield break;

            IsJumping = true;
            _jumpTime = jumpTimer;
            Body.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            var nextFixedUpdated = new WaitForFixedUpdate();

            while (IsJumping)
            {
                if (contextAction.ReadValue<float>() > 0 && _jumpTime > 0)
                {
                    Body.AddForce(
                        Vector2.up * (jumpPower * (_jumpTime / jumpTimer)),
                        ForceMode2D.Impulse);
                    _jumpTime -= Time.fixedDeltaTime;
                }

                var prevGrounded = _isGrounded;
                yield return nextFixedUpdated;
                if (!prevGrounded && _isGrounded)
                {
                    break;
                }
            }

            IsJumping = false;
        }

        private bool GroundCheck()
        {
            _groundCollision[0] = null;

            Physics2D.OverlapAreaNonAlloc(topLeftBound.position,
                bottomRightBound.position, _groundCollision, floorLayer);

            return _groundCollision.First();
        }
    }
}