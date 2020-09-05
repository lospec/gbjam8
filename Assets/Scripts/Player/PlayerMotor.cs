using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
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

        public UnityEvent<float> OnPlayerStartJump;
        public UnityEvent<float> OnPlayerJumpHeld;
		public UnityEvent<float> OnPlayerLand;

        private float _jumpTime;
        private bool _isGrounded;
        private Vector2 _gravity;
        private Collider2D[] _groundCollision = new Collider2D[1];

        public Rigidbody2D Body { get; private set; }
        public bool IsJumping { get; private set; }

        public bool IsAir => IsJumping || !_isGrounded;
        public Vector2 Move { get; set; }

		private Vector2 _lastVelocity;

        private void Awake()
        {
            Body = GetComponent<Rigidbody2D>();
            Body.gravityScale = 0;
        }

        private void OnDisable()
        {
            _gravity = Vector2.zero;
        }

        private void FixedUpdate()
        {
			bool wasGrounded = _isGrounded;
            _isGrounded = GroundCheck();

			if (!wasGrounded && _isGrounded)
			{
				OnPlayerLand?.Invoke(_lastVelocity.y);
			}

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

			_lastVelocity = Body.velocity;

		}


        public IEnumerator Jump(InputAction contextAction)
        {
            if (!_isGrounded || IsJumping) yield break;

            IsJumping = true;
            _jumpTime = jumpTimer;
            Body.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            var netJumpMagnitude = jumpPower;

            OnPlayerStartJump?.Invoke(netJumpMagnitude);

            var nextFixedUpdated = new WaitForFixedUpdate();

            while (IsJumping)
            {
                if (contextAction.ReadValue<float>() > 0 && _jumpTime > 0)
                {
                    float jumpMagnitude = jumpPower * (_jumpTime / jumpTimer);
                    Body.AddForce(
                        Vector2.up * jumpMagnitude,
                        ForceMode2D.Impulse);
                    _jumpTime -= Time.fixedDeltaTime;

                    netJumpMagnitude += jumpMagnitude;

                    OnPlayerJumpHeld?.Invoke(netJumpMagnitude);
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