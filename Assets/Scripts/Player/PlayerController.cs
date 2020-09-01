using Inputs;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapon.Hook;

namespace Player
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerController : MonoBehaviour, PlayerControls.IPlayerActions
    {
        private static class AnimParams
        {
            public static readonly int IsRunning = Animator.StringToHash("IsRunning");
            public static readonly int IsJumping = Animator.StringToHash("IsJumping");
        }

        [SerializeField] private GrapplingGun grapplingGun;


        private Animator _animator;
        private PlayerControls _input = default;
        private SpriteRenderer _spriteRenderer;
        private PlayerMotor _motor;


        private Vector2 _inputVector;

        private void Awake()
        {
            _input = new PlayerControls();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<Animator>();
            _motor = GetComponent<PlayerMotor>();
            grapplingGun.Motor = _motor;
        }

        private void Start()
        {
            _input.Player.Movement.performed += OnMovement;
            _input.Player.Primary.performed += OnPrimary;
            _input.Player.Secondary.performed += OnSecondary;
        }

        private void Update()
        {
            _motor.Move = new Vector2(_inputVector.x, _motor.Move.y);
            grapplingGun.Aim = _inputVector;
            UpdateSpriteAndAnimations();
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
            _inputVector = context.ReadValue<Vector2>();
        }

        public void OnPrimary(InputAction.CallbackContext context)
        {
            if (context.control.IsPressed())
                StartCoroutine(_motor.Jump(context.action));
        }


        public void OnSecondary(InputAction.CallbackContext context)
        {
            grapplingGun.PerformGrapple();
        }

        private void UpdateSpriteAndAnimations()
        {
            if (_inputVector.x != 0) _spriteRenderer.flipX = !(_inputVector.x >= 0);
            _animator.SetBool(AnimParams.IsJumping, _motor.IsJumping);
            _animator.SetBool(AnimParams.IsRunning,
                Mathf.Abs(_motor.Body.velocity.x) > 0.25f);
        }
    }
}