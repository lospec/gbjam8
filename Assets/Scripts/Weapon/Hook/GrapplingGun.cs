using System;
using Player;
using UnityEngine;

namespace Weapon.Hook
{
    public class GrapplingGun : MonoBehaviour
    {
        /// <summary> The speed of the hook when shot, use 0 for instant shot </summary>
        [Tooltip("The speed of the hook when shot, use 0 for instant shot")]
        public float shootSpeed = 0f;

        /// <summary>
        ///     The time it takes in seconds for the hook to return if it can't hit a target, this will only be used when
        ///     Shoot Speed and Max Hook Distance Speed is more than 0 (and not Infinity for the distance)
        /// </summary>
        [Tooltip(
            "The time it takes in seconds for the hook to return if it can't hit a target, this will only be used when Shoot Speed and Max Hook Distance Speed is more than 0 (and not Infinity for the distance)")]
        public float returnSpeed = 1f;

        /// <summary> The speed of the hook when shot, use 0 for instant shot </summary>
        [Tooltip("The grapples pulling speed")]
        public float pullSpeed = 50f;

        /// <summary> The maximum range the player can grapple, use 0 for infinite range </summary>
        [Tooltip("The maximum range the player can grapple, use 0 for infinite range")]
        public float maxHookDistance = 0f;

        [SerializeField] private Transform hook;

        private Vector2 _aim = Vector2.right;
        private Vector2 _previousAim;
        private float _lastDirection = 1f;
        private float _sameDirectionCooldown = 2f;
        private float _timer = 0f;

        private Vector2 HookPosition
        {
            set => hook.transform.position = value;
        }
        public Vector2 HookOrigin => Motor.Body.position;
        private Vector2 Target { get; set; }

        public PlayerMotor Motor { get; set; }

        public Vector2 Aim
        {
            get => _aim;
            set => _aim = new Vector2
            {
                x = Mathf.Abs(value.x) > 0f
                    ? new Func<float>(() =>
                    {
                        _lastDirection = Mathf.Sign(value.x);
                        return value.x;
                    }).Invoke()
                    : Mathf.Abs(value.y) > 0f
                        ? 0
                        : _lastDirection,
                y = value.y
            };
        }

        private void Start()
        {
            enabled = false;
        }

        private void Update()
        {
            if (_timer <= _sameDirectionCooldown)
            {
                _timer += Time.deltaTime;
            }
        }

        private void FixedUpdate()
        {
            if (Motor.enabled)
                DisableMovement();

            MoveTowardsTarget();
        }

        public static event Action<float, Vector2, Transform, Action> HookShot;
        public static event Action<float, Transform, Action> HookRetract;
        public static event Action HookRetractEnd;
        public static event Action EndPull;


        private void MoveTowardsTarget()
        {
            // if anybody have a better way to move the player, feel free to change my script
            var dir = (Target - HookOrigin).normalized;
            var vel = dir * pullSpeed;

            // Player hits an obstacle
            var hits = new RaycastHit2D[4];


            // Player reaches the target
            if (Motor.Body.OverlapPoint(Target) ||
                Motor.Body.Cast(dir, hits, pullSpeed * Time.fixedDeltaTime) > 0f)
            {
                EndPull?.Invoke();
                StopGrappling();
            }

            Motor.Body.AddForce(vel, ForceMode2D.Impulse);
        }

        private void StopGrappling()
        {
            enabled = false;
            EnableMovement();
            hook.SetParent(transform, true);
        }

        public void PerformGrapple()
        {
            if (_aim == _previousAim && enabled && _timer < _sameDirectionCooldown)
            {
                return;
            }

            _previousAim = _aim;
            _timer = 0;
            
            enabled = false;
            var hit = maxHookDistance > 0f
                ? Physics2D.Raycast(HookOrigin, _aim, maxHookDistance)
                : Physics2D.Raycast(HookOrigin, _aim);


            if (hit.collider)
            {
                Target = hit.point;
                HookPosition = shootSpeed > 0f ? HookOrigin : Target;
                HookShot?.Invoke(shootSpeed, Target, hook, () => { enabled = true; });
            }

            else if (shootSpeed > 0f && maxHookDistance > 0f &&
                     !float.IsPositiveInfinity(maxHookDistance))
            {
                EnableMovement();
                Target = HookOrigin + _aim.normalized * maxHookDistance;
                HookPosition = HookOrigin;
                HookShot?.Invoke(shootSpeed, Target, hook, () => HookRetract?.Invoke
                (returnSpeed, hook,
                    () =>
                    {
                        StopGrappling();
                        HookRetractEnd?.Invoke();
                    }));
            }
        }


        private void DisableMovement()
        {
            Motor.Body.velocity = Vector2.zero;
            Motor.enabled = false;
        }

        private void EnableMovement()
        {
            hook.SetParent(transform, true);
            // Motor.Body.velocity = Vector2.zero;
            Motor.enabled = true;
        }
    }
}