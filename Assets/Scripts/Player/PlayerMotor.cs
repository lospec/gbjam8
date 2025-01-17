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
        [Header("Moving")]
        public float moveSpeed = 10f;

        [Tooltip("The time it takes for the player to go from 0 to moveSpeed")]
        public float accelerationTime = .2f;
        [Tooltip("The time it takes for the player to go from moveSpeed to zero")]
        public float decelerationTime = 0f;

        [Header("Jumping")]
        public float maxJumpHeight = 5f;
        public float maxJumpTime = .1f;
        public float minAscensionTime;
        public float maxAscensionTime;

        [Header("Falling")]
        public float fallMultiplier = 2f;
        public float terminalVelocity = 50f;
        public float JumpGravity => 2f * maxJumpHeight / (maxJumpTime * maxJumpTime);
        public float FallGravity => JumpGravity * fallMultiplier;

        [SerializeField] private Transform topLeftBound = default;
        [SerializeField] private Transform bottomRightBound = default;

        public UnityEvent OnPlayerStartJump;
		public UnityEvent<float> OnPlayerLand;

        private float _jumpTime;
        private float jumpFinishedTime;
        private float minJumpFinishedTime;
        private bool isJumping = false;
        private bool _isGrounded;
        private Vector2 _gravity;
        private Collider2D[] _groundCollision = new Collider2D[1];

        public Rigidbody2D Body { get; private set; }
        public bool IsJumping { get; private set; }

        public bool IsAir => IsJumping || !_isGrounded;
        public Vector2 Move { get; set; }

        public float Acceleration
        {
            get => accelerationTime > 0f ? moveSpeed / accelerationTime : Mathf.Infinity;
            set => accelerationTime = moveSpeed > 0f ? value / moveSpeed : 0f;
        }

        public float Deceleration
        {
            get => decelerationTime > 0f ? moveSpeed / decelerationTime : Mathf.Infinity;
            set => decelerationTime = moveSpeed > 0f ? value / moveSpeed : 0f;
        }

		private Vector2 _lastVelocity;

        private void Awake()
        {
            Body = GetComponent<Rigidbody2D>();
            Body.gravityScale = 0;
        }

        private void OnDisable()
        {
            EndJump();
            _gravity = Vector2.zero;
        }

        private void FixedUpdate()
        {
			bool wasGrounded = _isGrounded;
            _isGrounded = GroundCheck();

            // Movement
            float targetSpeed = Move.x * moveSpeed;
            float diff = targetSpeed - Body.velocity.x;

            float maxMoveForce = Mathf.Abs(diff / Time.deltaTime);
            float moveForce = Math.Abs(diff) < Mathf.Abs(targetSpeed) ? Acceleration : Deceleration;

            if (moveForce <= 0f) moveForce = maxMoveForce;
            else moveForce = moveForce < maxMoveForce ? moveForce : maxMoveForce;

            moveForce *= Mathf.Sign(diff);
            Body.AddForce(new Vector2(moveForce, 0f) * Body.mass);

            // Apply Gravity
            //_gravity = _isGrounded
            //    ? Vector2.zero
            //    : _gravity + Vector2.down * fallingGravity;

            if (_jumpTime > 0f)
                _jumpTime -= Time.deltaTime;
                float gravity = _jumpTime > 0f ? JumpGravity : FallGravity;
                float maxGravity = (terminalVelocity + Body.velocity.y) / Time.deltaTime;

                if (gravity > maxGravity) gravity = maxGravity;

            if (!_isGrounded || _jumpTime > 0f)
            {
                Body.AddForce(new Vector2(0f, -gravity * Body.mass));
            }
            else
            {
                Body.velocity = new Vector2(Body.velocity.x, 0f);
            }

            //Vector2 velocity = Body.velocity;
            //velocity.x = Math.Abs(velocity.x) > moveSpeed ?
            //    Math.Sign(velocity.x) * moveSpeed :
            //    velocity.x;

            //Body.velocity = velocity;
            //Move = Vector2.zero;

			_lastVelocity = Body.velocity;

		}

        public void StartJump()
        {
            if (!_isGrounded) return;
            IsJumping = true;

            float jumpForce = Mathf.Sqrt(2f * maxJumpHeight * JumpGravity);
            Body.AddForce(new Vector2(0f, jumpForce) * Body.mass, ForceMode2D.Impulse);

            _jumpTime = maxJumpTime;

			OnPlayerStartJump?.Invoke();
		}

        public void EndJump()
        {
            IsJumping = false;
            _jumpTime = 0f;
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