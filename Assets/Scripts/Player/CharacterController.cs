using System;
using System.Collections;
using System.Linq;
using Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(SpriteRenderer),
        typeof(Rigidbody),
        typeof(PlayerInput))]
    public class CharacterController : MonoBehaviour, PlayerControls.IPlayerActions
    {
        private const float MoveSpeedModifier = 10f;

        public LayerMask floorLayer;

        public float moveSpeed = 10f,
            jumpPower = 30f,
            jumpTimer = 0.1f,
            customGravity = 9.88f;

        [SerializeField] private Animator animator = default;
        [SerializeField] private Transform topLeftBound = default;
        [SerializeField] private Transform bottomRightBound = default;


        private float _coyoteTime, _jumpTime;
        private PlayerControls _input = default;
        private bool _isGrounded, _jumping;


        private Vector2 _moveVector, _currentForceOfGravity;

        private Rigidbody2D _rigidbody;
        private SpriteRenderer _spriteRenderer;

        private Collider2D[] _groundCollision = new Collider2D[1];

        private void Awake()
        {
            _input = new PlayerControls();

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody = GetComponent<Rigidbody2D>();

            //We use custom gravity to ensure the smoothest terrain traversal possible.
            _rigidbody.gravityScale = 0;
        }

        private void Start()
        {
            _input.Player.Movement.performed += OnMovement;
            _input.Player.Movement.canceled += _ => _moveVector = Vector2.zero;
            _input.Player.Primary.started += OnPrimary;
            _input.Player.Secondary.started += OnSecondary;
        }

        private void Update()
        {
            _coyoteTime += Time.deltaTime;
            UpdateSpriteAndAnimations();
        }


        private void FixedUpdate()
        {
            _isGrounded = GroundCheck();
            _currentForceOfGravity = _isGrounded
                ? Vector2.zero
                : _currentForceOfGravity + Vector2.down * customGravity;

            _rigidbody.AddForce(_currentForceOfGravity +
                                _moveVector * (moveSpeed * MoveSpeedModifier));
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            _moveVector = Vector2.zero;
            _moveVector.x = context.ReadValue<Vector2>().x;
        }

        public void OnPrimary(InputAction.CallbackContext context)
        {
            if (context.control.IsPressed()) StartCoroutine(Jump(context.action));
        }


        public void OnSecondary(InputAction.CallbackContext context)
        {
            Hook();
        }

        private void Hook()
        {
            throw new NotImplementedException();
        }

        private IEnumerator Jump(InputAction contextAction)
        {
            if (!_isGrounded || _jumping) yield break;

            _jumping = true;
            _jumpTime = jumpTimer;
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            var nextFixedUpdated = new WaitForFixedUpdate();
            while (_jumping && contextAction.ReadValue<float>() > 0 && _jumpTime > 0)
            {
                var prevGrounded = _isGrounded;
                yield return nextFixedUpdated;
                _rigidbody.velocity += Vector2.up * jumpPower;
                _jumpTime -= Time.fixedDeltaTime;

                if (!prevGrounded && _isGrounded)
                {
                    Debug.Log("break jump");
                    break;
                }
            }

            _jumping = false;
        }

        private void UpdateSpriteAndAnimations()
        {
            if (_moveVector.x != 0) _spriteRenderer.flipX = !(_moveVector.x >= 0);

            //Plug in animation setfloats and setbools here!
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