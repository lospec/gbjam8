using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class CharacterController : MonoBehaviour
    {
        public Direction direction = Direction.E;
        public LayerMask
            floorLayer; //Use this to set which layers will be checked for colliders to see whether the character is grounded.

        //Create two empties and position them at the bottom corners of the character for the groundcheck.
        public Transform leftFoot, rightFoot;
        public float moveSpeed = 3000f,
            jumpPower = 5000f,
            maxCoyoteTime = .15f,
            customGravity = 1000f;

        //Graphics related
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Animator animator;
        private float _coyoteTime;
        private bool _isGrounded, _jumpIsQueued;

        //Movement & Jumping
        private Vector2 _moveVector, _currentForceOfGravity;
        //General Properties
        private Rigidbody2D _rigidbody;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            _rigidbody = GetComponent<Rigidbody2D>();

            _rigidbody.gravityScale =
                0; //We use custom gravity to ensure the smoothest terrain traverlsal possible.
        }

        private void Update()
        {
            _coyoteTime += Time.deltaTime;
            ReadControls();
            UpdateSpriteAndAnimations();
        }

        private void FixedUpdate()
        {
            _isGrounded = GroundCheck();
            CalcGravity();

            if (_jumpIsQueued)
            {
                _rigidbody.AddForce(Vector2.up * (jumpPower * Time.fixedDeltaTime),
                    ForceMode2D.Impulse);
                _jumpIsQueued = false;
            }

            _rigidbody.AddForce(
                (_moveVector + _currentForceOfGravity) * Time.fixedDeltaTime);
        }

        private void ReadControls()
        {
            _moveVector = Vector2.zero;

            // Movement
            if (Keyboard.current.leftArrowKey.isPressed)
            {
                _moveVector.x = -moveSpeed;
                if (Keyboard.current.upArrowKey.isPressed) direction = Direction.NW;
                else if (Keyboard.current.downArrowKey.isPressed)
                    direction = Direction.SW;
                else direction = Direction.W;
            }
            else if (Keyboard.current.rightArrowKey.isPressed)
            {
                _moveVector.x = moveSpeed;
                if (Keyboard.current.upArrowKey.isPressed) direction = Direction.NE;
                else if (Keyboard.current.downArrowKey.isPressed)
                    direction = Direction.SE;
                else direction = Direction.E;
            }

            if (_moveVector.x != 0 && Keyboard.current.upArrowKey.isPressed)
                direction = Direction.N;
            else if (_moveVector.x != 0 && Keyboard.current.downArrowKey.isPressed)
                direction = Direction.S;

            // A-Button
            // This is a bool so the input can not be read multiple times before the next physics frame (even if this is very unlikely to happen).
            if (Keyboard.current.xKey.wasPressedThisFrame && _isGrounded &&
                !_jumpIsQueued)
                _jumpIsQueued = true;

            // B-Button
            if (Keyboard.current.cKey.wasPressedThisFrame)
                Hook(); //This should live in its own script.
        }

        private void Hook()
        {
            throw new NotImplementedException();
        }

        private void UpdateSpriteAndAnimations()
        {
            switch (direction)
            {
                case Direction.E:
                case Direction.NE:
                case Direction.SE:
                    sr.flipX = false;
                    break;
                case Direction.W:
                case Direction.NW:
                case Direction.SW:
                    sr.flipX = true;
                    break;
            }

            //Plug in animation setfloats and setbools here!
        }

        private bool GroundCheck()
        {
            if (Physics2D.Linecast(transform.position, leftFoot.position, floorLayer)
                || Physics2D.Linecast(transform.position, rightFoot.position,
                    floorLayer))
            {
                _coyoteTime = 0f;
                return true;
            }

            return _coyoteTime <= maxCoyoteTime;
        }

        private void CalcGravity()
        {
            if (!_isGrounded)
                _currentForceOfGravity.y -= customGravity * Time.fixedDeltaTime;
            else _currentForceOfGravity = Vector2.zero;
        }
    }
}