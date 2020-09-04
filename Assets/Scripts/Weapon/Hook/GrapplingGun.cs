﻿using System;
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

        /// <summary> The time it takes for the grapple to pull at max speed </summary>
        [Tooltip("The time it takes for the grapple to pull at max speed")]
        public float pullAccelerationTime = .2f;

        /// <summary> The time it takes for the grapple to pull at max speed </summary>
        [Tooltip("The time it takes for the grapple to pull at max speed")]
        public float drag = .2f;

        /// <summary> The maximum range the player can grapple, use 0 for infinite range </summary>
        [Tooltip("The maximum range the player can grapple, use 0 for infinite range")]
        public float maxHookDistance = 0f;

        public float stuckCheckThreshold = .01f;
        public float stuckTimeThreshold = .01f;

        [SerializeField] private Transform hook;

        private Vector2 _aim = Vector2.right;
        private Vector2 _previousAim;
        private float _lastDirection = 1f;
        private float _sameDirectionCooldown = 2f;
        private float _sameDirectionTimer = 0f;
        private float _stuckTime = 0f;

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
            if (_sameDirectionTimer <= _sameDirectionCooldown)
            {
                _sameDirectionTimer += Time.deltaTime;
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
            // Drag
            float velMag = Motor.Body.velocity.magnitude;
            float maxDrag = velMag / Time.deltaTime;

            float dragMag = velMag * velMag * drag;
            if (dragMag > maxDrag) dragMag = maxDrag;

            Vector2 velNormalized = Motor.Body.velocity;
            if (velMag > 0f) velNormalized /= velMag;
            Vector2 dragForce = dragMag * -velNormalized;
            Debug.Log(dragMag);
            Motor.Body.AddForce(dragForce);

            Vector2 dir = (Target - HookOrigin).normalized;
            float dot = Vector2.Dot(Motor.Body.velocity, dir);
            float diff = pullSpeed - dot;
            float maxPullForce = diff / Time.deltaTime;
            float tension = Mathf.Max(0f, -dot) / Time.deltaTime;

            float pullForce = (pullAccelerationTime > 0f) ?
                pullSpeed / pullAccelerationTime + tension :
                maxPullForce;

            pullForce = (pullForce < maxPullForce) ? pullForce : maxPullForce;
            Motor.Body.AddForce(dir * pullForce * Motor.Body.mass);

            // Player hits an obstacle
            var hits = new RaycastHit2D[4];
            bool hitObject = Motor.Body.Cast(dir, hits, pullSpeed * Time.fixedDeltaTime) > 0f;

            // Stuck Check
            bool approximatelyStuck = false;
            if (hitObject) approximatelyStuck = Motor.Body.velocity.sqrMagnitude <= stuckCheckThreshold;
            if (approximatelyStuck) _stuckTime += Time.deltaTime;
            else _stuckTime = 0f;
            approximatelyStuck = _stuckTime >= stuckTimeThreshold;

            // Player reaches the target
            bool collideNearTarget = hitObject && (Target - HookOrigin).sqrMagnitude <= 1.0025f;
            bool reachesTarget = Motor.Body.OverlapPoint(Target);

            if (reachesTarget || collideNearTarget || approximatelyStuck)
            {
                if (hitObject)
                    Motor.Body.velocity = Vector2.zero;

                OnPullEnded?.Invoke();
                StopGrappling();
            }
        }

        private void StopGrappling()
        {
            enabled = false;
            EnableMovement();
            hook.SetParent(transform, true);
        }

        public void PerformGrapple()
        {
            if (_aim == _previousAim && enabled && _sameDirectionTimer < _sameDirectionCooldown)
            {
                return;
            }

            _previousAim = _aim;
            _sameDirectionTimer = 0;

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