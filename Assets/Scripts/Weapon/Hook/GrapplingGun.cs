using System;
using Player;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using System.Collections.Generic;
#if UNITY_EDITOR
using Handles = UnityEditor.Handles;
#endif
using UnityEngine.Events;

namespace Weapon.Hook
{
    public class GrapplingGun : MonoBehaviour
    {
        [SerializeField] private LayerMask hookableMask = default;
        [SerializeField] private LayerMask autoAimMask = default;

        [Header("Hook")]
        [SerializeField] private Transform hook;

        /// <summary> The speed of the hook when shot, use 0 for instant shot </summary>
        [Tooltip("The speed of the hook when shot, use 0 for instant shot")]
        public float shootSpeed = 0f;

        /// <summary>
        ///     The retraction speed of the hook in unit distance per seconds, this will only be used when
        ///     Shoot Speed and Max Hook Distance Speed is more than 0 (and not Infinity for the distance)
        /// </summary>
        [Tooltip(
            "The retraction speed of the hook in unit distance per seconds, this will only be used when Shoot Speed and Max Hook Distance Speed is more than 0 (and not Infinity for the distance)")]
        public float retractSpeed = 1f;

        [Header("Grapple")]
        /// <summary> The speed of the hook when shot, use 0 for instant shot </summary>
        [Tooltip("The grapples pulling speed")]
        public float pullSpeed = 50f;

        /// <summary> The time it takes for the grapple to pull at max speed </summary>
        [Tooltip("The time it takes for the grapple to pull at max speed")]
        public float pullAccelerationTime = .2f;

        /// <summary> The time it takes for the grapple to pull at max speed </summary>
        [Tooltip("The time it takes for the grapple to pull at max speed")]
        public float drag = .2f;

        [Header("Checks")]
        /// <summary> The maximum range the player can grapple, use 0 for infinite range </summary>
        [Tooltip("The maximum range the player can grapple, use 0 for infinite range")]
        public float maxShootDistance = 0f;

        /// <summary> The minimum distance from the gunpoint(see: <see cref="HookOrigin"/>) to the target for it count the player has arrived at the target </summary>
        [Tooltip(
            "The minimum distance from the gunpoint to the target for it count that the player has arrived at the target")]
        public float arrivedDistance = 5f;

        public float stuckCheckThreshold = .01f;
        public float stuckTimeThreshold = .01f;

        [Header("Auto-Aim")]
        public bool autoAimEnabled = true;

        /// <summary> Angle in which the auto-aim checks for target in degrees, kind of like Field of View angle </summary>
        [Tooltip(
            "Angle in which the auto-aim checks for target in degrees, kind of like Field of View angle")]
        [SerializeField] float autoAimAngle = 360f / 8f;

        /// <summary> Minimum distance the player will auto-aim to a target </summary>
        [Tooltip("Minimum distance the player will auto-aim to a target")]
        [SerializeField] float autoAimRange = 20f;

        private Vector2 _aimInput = Vector2.right;
        private Vector2 _previousAim;
        private float _lastDirection = 1f;
        private float _sameDirectionCooldown = 2f;
        private float _sameDirectionTimer = 0f;
        private float _stuckTime = 0f;
        private bool _grappleEnabled = true;

        public bool GrappleEnabled
        {
            get => _grappleEnabled;
            set
            {
                if (value == false)
                    StopGrappling();

                _grappleEnabled = value;
            }
        }

        private Vector2 HookPosition
        {
            get => hook.position;
            set => hook.position = value;
        }
        public Vector2 HookOrigin => transform.position;

        public float AutoAimAngle_Rad
        {
            get => autoAimAngle * Mathf.Deg2Rad;
            set => autoAimAngle = value * Mathf.Rad2Deg;
        }
        public float AutoAimAngle_Deg
        {
            get => autoAimAngle;
            set => autoAimAngle = value;
        }

        private Vector2 Target { get; set; }
        public Collider2D TargetObject { get; set; }

        public PlayerMotor Motor { get; set; }

        public Vector2 AimInput
        {
            get => _aimInput;
            set
            {
                _aimInput.y = value.y;
                _aimInput.x = (Mathf.Abs(value.y) > 0f) ? 0 : _lastDirection;

                if (Mathf.Abs(value.x) > 0f)
                {
                    _lastDirection = Mathf.Sign(value.x);
                    _aimInput.x = value.x;
                }

                //x = Mathf.Abs(value.x) > 0f
                //    ? new Func<float>(() =>
                //    {
                //        _lastDirection = Mathf.Sign(value.x);
                //        return value.x;
                //    }).Invoke()
                //    : Mathf.Abs(value.y) > 0f
                //        ? 0
                //        : _lastDirection
            }
        }

        public Vector2 Aim
        {
            get
            {
                if (autoAimEnabled)
                {
                    Collider2D target = GetAutoAimTarget();

                    if (target != null)
                        return ((Vector2) target.transform.position - HookOrigin)
                            .normalized;
                }

                return AimInput;
            }
        }

        public delegate void HookShotDelegate(float speed, Vector2 target,
            Transform hook, Action finishShooting);

        public delegate void HookTargetHitDelegate(Vector2 hookPosition,
            Collider2D targetObject);

        public delegate void RetractHookDelegate(float retractDuration, Transform
            hook, Action finishRetracting);

        public delegate void HookRetractedDelegate();

        public delegate void PullEndedDelegate(bool arrivedAtTarget,
            Collider2D targetObject, Collider2D collidedObject);

        public static event HookShotDelegate OnHookShot;
        public static event HookTargetHitDelegate OnHookTargetHit;
        public static event RetractHookDelegate OnRetractHook;
        public static event HookRetractedDelegate OnHookRetracted;
        public static event PullEndedDelegate OnPullEnded;

        public UnityEvent OnHookFired;
        public UnityEvent OnHookMapShot;
        public UnityEvent OnHookEnemyShot;

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
            if (!GrappleEnabled)
            {
                if (enabled) StopGrappling();
                return;
            }

            if (Motor.enabled)
                DisableMovement();

            MoveTowardsTarget();
        }


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

            Motor.Body.AddForce(dragForce);

            Vector2 dir = (Target - HookOrigin).normalized;
            float dot = Vector2.Dot(Motor.Body.velocity, dir);
            float diff = pullSpeed - dot;
            float maxPullForce = diff / Time.deltaTime;
            float tension = Mathf.Max(0f, -dot) / Time.deltaTime;

            float pullForce = (pullAccelerationTime > 0f)
                ? pullSpeed / pullAccelerationTime + tension
                : maxPullForce;

            pullForce = (pullForce < maxPullForce) ? pullForce : maxPullForce;
            Motor.Body.AddForce(dir * pullForce * Motor.Body.mass);

            // Player hits an obstacle
            var hits = new RaycastHit2D[1];
            bool hitObject =
                Motor.Body.Cast(dir, hits, pullSpeed * Time.fixedDeltaTime) > 0f;

            // Stuck Check
            bool approximatelyStuck = false;
            if (hitObject)
                approximatelyStuck =
                    Motor.Body.velocity.sqrMagnitude <= stuckCheckThreshold;
            if (approximatelyStuck) _stuckTime += Time.deltaTime;
            else _stuckTime = 0f;
            approximatelyStuck = _stuckTime >= stuckTimeThreshold;

            // Player reaches the target
            bool arrived = (Target - HookOrigin).sqrMagnitude <=
                           arrivedDistance * arrivedDistance;
            bool collideNearTarget = hitObject && arrived;
            bool reachesTarget = Motor.Body.OverlapPoint(Target);

            if (reachesTarget || collideNearTarget || approximatelyStuck)
            {
                if (hitObject)
                    Motor.Body.velocity = Vector2.zero;

                Collider2D collidedObject = hits[0] ? hits[0].collider : null;
                StopGrappling();

                OnPullEnded?.Invoke(arrived, TargetObject, collidedObject);

                // Resets Target just in case
                TargetObject = null;
            }
        }

        private void StopGrappling()
        {
            enabled = false;
            EnableMovement();

            HookPosition = HookOrigin;
            hook.SetParent(transform, true);
        }

        #region Auto-Aim

        private Collider2D GetAutoAimTarget()
        {
            //if (maxShootDistance <= 0f || maxShootDistance >= Mathf.Infinity)
            //    return AutoAim_Raycasts(180);

            return AutoAim_OverlapCircle();
        }

        private Collider2D AutoAim_Raycasts(int rayCount)
        {
            float nearestDist = Mathf.Infinity;
            Collider2D nearestTarget = null;

            for (int i = 1; i <= rayCount; i++)
            {
                float a = 0f;
                if (rayCount > 1)
                    a = a - autoAimAngle / 2f + autoAimAngle / rayCount * i;
                a *= Mathf.Deg2Rad;

                float sin = Mathf.Sin(a);
                float cos = Mathf.Cos(a);
                float tx = AimInput.x;
                float ty = AimInput.y;

                Vector2 aim = new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);

                RaycastHit2D hit =
                    Physics2D.Raycast(HookOrigin, aim, autoAimRange, hookableMask);
                if (hit && (1 << hit.collider.gameObject.layer & autoAimMask) != 0)
                {
                    bool newNear = hit.distance < nearestDist;
                    nearestDist = newNear ? hit.distance : nearestDist;
                    nearestTarget = newNear ? hit.collider : nearestTarget;
                }
            }

            return nearestTarget;
        }

        private Collider2D AutoAim_OverlapCircle()
        {
            Collider2D[] colliders =
                Physics2D.OverlapCircleAll(HookOrigin, autoAimRange, hookableMask);
            float nearestDist = Mathf.Infinity;
            Collider2D nearestTarget = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                Transform target = colliders[i].transform;
                Vector2 targetAim = (Vector2) target.position - HookOrigin;

                if ((1 << target.gameObject.layer & autoAimMask) == 0) continue;
                if (Vector2.Angle(AimInput, targetAim) > AutoAimAngle_Deg / 2f)
                    continue;

                RaycastHit2D hit = Physics2D.Raycast(HookOrigin, targetAim,
                    autoAimRange, hookableMask);
                if (hit.transform != target) continue;
                if (hit.distance >= nearestDist) continue;

                nearestDist = hit.distance;
                nearestTarget = hit.collider;
            }

            return nearestTarget;
        }

        #endregion

        public Vector2 FindTargetPosition(out RaycastHit2D hit, out bool targetHit)
        {
            targetHit = false;
            Vector2 aim = Aim;
            hit = maxShootDistance > 0f
                ? Physics2D.Raycast(HookOrigin, aim, maxShootDistance, hookableMask)
                : Physics2D.Raycast(HookOrigin, aim, Mathf.Infinity, hookableMask);
            if (hit.collider)
            {
                Target = hit.point;
                targetHit = true;
            }
            else if (shootSpeed > 0f && maxShootDistance > 0f &&
                     !float.IsPositiveInfinity(maxShootDistance))
            {
                Target = HookOrigin + Aim.normalized * maxShootDistance;
                targetHit = false;
            }

            return Target;
        }

        public void PerformGrapple()
        {
            Vector2 aim = Aim;

            // the player shot in the same direction and the player is using the grappling(plus some cooldown)
            // Don't do anything
            if (aim == _previousAim && enabled &&
                _sameDirectionTimer < _sameDirectionCooldown)
            {
                return;
            }

            _previousAim = aim;
            _sameDirectionTimer = 0;

            bool prevEnabled = enabled;
            enabled = false;

            Target = FindTargetPosition(out var hit, out var targetHit);
            // If hook hit something
            if (targetHit)
            {
                HookHit(hit, prevEnabled);
            }
            else
            {
                HookMiss(aim);
            }
        }

        private void HookMiss(Vector2 aim)
        {
            EnableMovement();
            Target = HookOrigin + aim.normalized * maxShootDistance;
            HookPosition = HookOrigin;

            // Start shooting the hook
            OnHookShot?.Invoke(shootSpeed, Target, hook, () =>
            {
                // When the shot is finished(animation finished, etc)

                // Start retracting the hook
                OnRetractHook?.Invoke(retractSpeed, hook, () =>
                {
                    // When the hook finished retracting

                    // Stop grappling(if it's enabled) and invoke OnHookRetracted
                    StopGrappling();
                    OnHookRetracted?.Invoke();
                });
            });
        }

        private void HookHit(RaycastHit2D hit, bool prevEnabled)
        {
            TargetObject = hit.collider;
            HookPosition = shootSpeed > 0f ? HookOrigin : Target;

            // If the hook is enabled(the player is grappling)
            if (prevEnabled)
            {
                // Retract the hook first, then 
                OnRetractHook?.Invoke(retractSpeed, hook, () =>
                {
                    // When the hook finished retracting
                    OnHookRetracted?.Invoke();

                    // THEN, Start shooting the hook
                    ShootHook();
                });
            }

            else ShootHook();
        }

        // HACK: Need a better system to manage the grappling hook shoot,retract,etc
        private void ShootHook()
        {
            HookPosition = shootSpeed > 0f ? HookOrigin : Target;

            // Start shooting the hook
            OnHookFired?.Invoke();
            OnHookShot?.Invoke(shootSpeed, Target, hook, () =>
            {
                // When the shot is landed(animation finished, etc)

                // enable grapple pull and invoke OnHookTargetHit
                enabled = true;

                // also make the hook stuck on target object(if any)
                //if (TargetObject != null)

                OnHookTargetHit?.Invoke(HookPosition, TargetObject);
                // HACK
                if (TargetObject.CompareTag("Enemy"))
                {
                    OnHookEnemyShot?.Invoke();
                    hook.SetParent(TargetObject.transform, true);
                }
                else
                {
                    OnHookMapShot?.Invoke();
                }
            });
        }


        private void DisableMovement()
        {
            Motor.Body.velocity = Vector2.zero;
            Motor.enabled = false;
        }

        private void EnableMovement()
        {
            // hook.SetParent(transform, true);
            // Motor.Body.velocity = Vector2.zero;
            Motor.enabled = true;
        }

        #region Gizmos and Debug

#if UNITY_EDITOR
        [System.Serializable]
        public struct GizmoOptions
        {
            public bool grappleRange;
            public bool arrivalDistance;
            public bool autoAimTarget;
            public bool autoAimRays;
            public bool limitRayCount;
        }

        [Header("GIZMOS/DEBUG")]
        public bool showGizmos = true;
        public Vector2 gizmosAim => AimInput;

        public GizmoOptions gizmoOptions = new GizmoOptions()
        {
            grappleRange = false,
            arrivalDistance = false,
            autoAimTarget = true,
            autoAimRays = false,
            limitRayCount = true,
        };

        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;

            if (gizmoOptions.arrivalDistance)
            {
                Handles.color = new Color(0f, 1f, 1f, .025f);
                Handles.DrawSolidDisc(HookOrigin, Vector3.forward, arrivedDistance);
            }

            if (gizmoOptions.grappleRange)
            {
                Handles.color = new Color(0f, 1f, 0f, .05f);
                Handles.DrawSolidDisc(HookOrigin, Vector3.forward, maxShootDistance);
            }

            if (gizmoOptions.autoAimRays)
            {
                Handles.color = new Color(1f, 0f, 0f, .5f);

                rayCount = rayCount <= 0 ? 1 : rayCount;
                if (gizmoOptions.limitRayCount)
                    rayCount = Mathf.Min(rayCount, (int) (autoAimAngle / .5f));

                for (int i = 1; i <= rayCount; i++)
                {
                    //Handles.color = Color.HSVToRGB((float)i / rayCount / 4f, 1f, 1f);

                    float a = 0f;
                    if (rayCount > 1)
                        a = a - autoAimAngle / 2f + autoAimAngle / rayCount * i;
                    a *= Mathf.Deg2Rad;

                    float sin = Mathf.Sin(a);
                    float cos = Mathf.Cos(a);
                    float tx = AimInput.x;
                    float ty = AimInput.y;

                    Vector2 aim = new Vector2(cos * tx - sin * ty, sin * tx + cos * ty);

                    RaycastHit2D hit = Physics2D.Raycast(HookOrigin, aim, autoAimRange);
                    Handles.DrawLine(HookOrigin,
                        hit ? hit.point : HookOrigin + aim * autoAimRange);
                }

                //Handles.DrawSolidArc(HookOrigin, Vector3.forward, gizAim, aimAngle, 1f);
            }

            if (gizmoOptions.autoAimTarget)
            {
                Handles.color = new Color(0f, 1f, 0f, .25f);

                Collider2D target = GetAutoAimTarget();
                if (target != null)
                    Handles.DrawLine(HookOrigin, target.transform.position);
            }
        }

        public int rayCount = 2;
#endif

        #endregion
    }
}