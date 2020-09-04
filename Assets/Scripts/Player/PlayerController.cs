using Inputs;
using System.Collections;
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
            public static readonly int IsAir = Animator.StringToHash("IsAir");
            public static readonly int AbsDirX = Animator.StringToHash("AbsDirX");
            public static readonly int DirY = Animator.StringToHash("DirY");
        }

        [SerializeField] private GrapplingGun grapplingGun;

        private Animator _animator;
        private PlayerControls _input = default;
        private SpriteRenderer _spriteRenderer;
        private PlayerMotor _motor;
        public bool isKnockingBack;

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

            isKnockingBack = false;
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
            _animator.SetBool(AnimParams.IsAir, _motor.IsAir);
            _animator.SetBool(AnimParams.IsRunning,
                Mathf.Abs(_motor.Body.velocity.x) > 0.25f);
            _animator.SetFloat(AnimParams.DirY, _inputVector.y);
            _animator.SetFloat(AnimParams.AbsDirX, Mathf.Abs(_inputVector.x));
        }

        public IEnumerator Knockback(float duration, float strength, bool fromRight)
        {
            this.enabled = false;
            isKnockingBack = true;

            _motor.Body.velocity = Vector3.zero;
            _motor.Body.AddForce(new Vector3(!fromRight ? -1 : 1, 1, 0) * strength, ForceMode2D.Impulse);

            yield return new WaitForSeconds(duration);

            isKnockingBack = false;
            this.enabled = true;
        }
    }
}