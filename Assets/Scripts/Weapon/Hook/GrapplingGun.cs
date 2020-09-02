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
        public float returnDuration = 1f;

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
        private GameObject TargetObject { get; set; }

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

        public delegate void HookShotDelegate(float speed, Vector2 target,
            Transform hook, Action finishShooting);

        public delegate void HookTargetHitDelegate();

        public delegate void RetractHookDelegate(float retractDuration, Transform
            hook, Action finishRetracting);

        public delegate void HookRetractedDelegate();

        public delegate void PullEndedDelegate();

        public static event HookShotDelegate OnHookShot;
        public static event HookTargetHitDelegate OnHookTargetHit;
        public static event RetractHookDelegate OnRetractHook;
        public static event HookRetractedDelegate OnHookRetracted;
        public static event PullEndedDelegate OnPullEnded;


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
                OnPullEnded?.Invoke();
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


            // If hook hit something
            if (hit.collider)
            {
                Target = hit.point;
                HookPosition = shootSpeed > 0f ? HookOrigin : Target;

                // Start shooting the hook
                OnHookShot?.Invoke(shootSpeed, Target, hook, () =>
                {
                    // When the shot is landed(animation finished, etc)

                    // enable grapple pull and invoke OnHookTargetHit
                    enabled = true;
                    OnHookTargetHit?.Invoke();
                });
            }

            else if (shootSpeed > 0f && maxHookDistance > 0f &&
                     !float.IsPositiveInfinity(maxHookDistance))
            {
                EnableMovement();
                Target = HookOrigin + _aim.normalized * maxHookDistance;
                HookPosition = HookOrigin;

                // Start shooting the hook
                OnHookShot?.Invoke(shootSpeed, Target, hook, () =>
                {
                    // When the shot is finished(animation finished, etc)

                    // Start retracting the hook
                    OnRetractHook?.Invoke(returnDuration, hook, () =>
                    {
                        // When the hook finished retracting

                        // Stop grappling(if it's enabled) and invoke OnHookRetracted
                        StopGrappling();
                        OnHookRetracted?.Invoke();
                    });
                });
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