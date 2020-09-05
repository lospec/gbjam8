using System;
using Player;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

#if UNITY_EDITOR
using Handles = UnityEditor.Handles;
#endif
using UnityEngine.Events;

namespace Weapon.Hook
{
    public class GrapplingGun : MonoBehaviour
    {
        [SerializeField] private LayerMask hookableLayers = default;

        /// <summary> The speed of the hook when shot, use 0 for instant shot </summary>
        [Tooltip("The speed of the hook when shot, use 0 for instant shot")]
        public float shootSpeed = 0f;

        /// <summary>
        ///     The retraction speed of the hook in unit distance per seconds, this will only be used when
        ///     Shoot Speed and Max Hook Distance Speed is more than 0 (and not Infinity for the distance)
        /// </summary>
        [Tooltip("The retraction speed of the hook in unit distance per seconds, this will only be used when Shoot Speed and Max Hook Distance Speed is more than 0 (and not Infinity for the distance)")]
        public float retractSpeed = 1f;

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

        /// <summary> The minimum distance from the gunpoint(see: <see cref="HookOrigin"/>) to the target for it count the player has arrived at the target </summary>
        [Tooltip("The minimum distance from the gunpoint to the target for it count that the player has arrived at the target")]
        public float arrivedDistance = 5f;

        public float stuckCheckThreshold = .01f;
        public float stuckTimeThreshold = .01f;

        [SerializeField] private Transform hook;

        private Vector2 _aim = Vector2.right;
        private Vector2 _previousAim;
        private float _lastDirection = 1f;
        private float _sameDirectionCooldown = 2f;
        private float _sameDirectionTimer = 0f;
        private float _stuckTime = 0f;
        private bool _grappleEnabled = true;
        private int _ammo = 0;

        public int Ammo
        {
            get => _ammo;
            set
            {
                int prev = _ammo;
                _ammo = value;

                OnAmmoUpdate?.Invoke(prev);
            }
        }

        public bool Enabled
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
        private Vector2 Target { get; set; }
        public Collider2D TargetObject { get; set; }

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
            if (!Enabled)
            {
                if (enabled) StopGrappling();
                return;
            }

            if (Motor.enabled)
                DisableMovement();

            MoveTowardsTarget();
        }

        public delegate void HookShotDelegate(float speed, Vector2 target,
            Transform hook, Action finishShooting);

        public delegate void HookTargetHitDelegate(Vector2 hookPosition, Collider2D targetObject);

        public delegate void RetractHookDelegate(float retractDuration, Transform
            hook, Action finishRetracting);

        public delegate void HookRetractedDelegate();

        public delegate void PullEndedDelegate(bool arrivedAtTarget, Collider2D targetObject, Collider2D collidedObject);
        
        public delegate void ShootEmptyDelegate();

        public delegate void AmmoUpdateDelegate(int previousValue);

        public static event HookShotDelegate OnHookShot;
        public static event HookTargetHitDelegate OnHookTargetHit;
        public static event RetractHookDelegate OnRetractHook;
        public static event HookRetractedDelegate OnHookRetracted;
        public static event PullEndedDelegate OnPullEnded;
        public static event ShootEmptyDelegate OnShootEmpty;
        public static event AmmoUpdateDelegate OnAmmoUpdate;

        public UnityEvent OnHookFired;
        public UnityEvent OnHookMapShot;
        public UnityEvent OnHookEnemyShot;

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

            float pullForce = (pullAccelerationTime > 0f) ?
                pullSpeed / pullAccelerationTime + tension :
                maxPullForce;

            pullForce = (pullForce < maxPullForce) ? pullForce : maxPullForce;
            Motor.Body.AddForce(dir * pullForce * Motor.Body.mass);

            // Player hits an obstacle
            var hits = new RaycastHit2D[1];
            bool hitObject = Motor.Body.Cast(dir, hits, pullSpeed * Time.fixedDeltaTime) > 0f;

            // Stuck Check
            bool approximatelyStuck = false;
            if (hitObject) approximatelyStuck = Motor.Body.velocity.sqrMagnitude <= stuckCheckThreshold;
            if (approximatelyStuck) _stuckTime += Time.deltaTime;
            else _stuckTime = 0f;
            approximatelyStuck = _stuckTime >= stuckTimeThreshold;

            // Player reaches the target
            bool arrived = (Target - HookOrigin).sqrMagnitude <= arrivedDistance * arrivedDistance;
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
            hook.SetParent(transform, true);
        }

        public void PerformGrapple()
        {
            // the player shot in the same direction and the player is using the grappling(plus some cooldown)
            // Don't do anything
            if (_aim == _previousAim && enabled && _sameDirectionTimer < _sameDirectionCooldown)
            {
                return;
            }

            if (Ammo <= 0)
            {
                OnShootEmpty?.Invoke();
                return;
            }

            _previousAim = _aim;
            _sameDirectionTimer = 0;
            Ammo--;

            bool prevEnabled = enabled;
            enabled = false;
            var hit = maxHookDistance > 0f
                ? Physics2D.Raycast(HookOrigin, _aim, maxHookDistance, hookableLayers)
                : Physics2D.Raycast(HookOrigin, _aim, Mathf.Infinity, hookableLayers);

            Target = hit.point;

            // If hook hit something
            if (hit.collider)
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
                        HookPosition = shootSpeed > 0f ? HookOrigin : Target;

                        // Start shooting the hook
                        OnHookFired?.Invoke();
                        OnHookShot?.Invoke(shootSpeed, Target, hook, () =>
                        {
                            // When the shot is landed(animation finished, etc)

                            // enable grapple pull and invoke OnHookTargetHit
                            enabled = true;

                            // also make the hook stuck on target object(if any)
                            if (TargetObject != null)
                                hook.SetParent(TargetObject.transform, true);

                            OnHookTargetHit?.Invoke(HookPosition, TargetObject);

                            if (hit.transform.gameObject.tag == "Enemy")
                            {
                                OnHookEnemyShot?.Invoke();
                            }
                            else
                            {
                                OnHookMapShot?.Invoke();
                            }
                        });
                    });
                }
                else
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
                        if (TargetObject != null)
                            hook.SetParent(TargetObject.transform, true);

                        OnHookTargetHit?.Invoke(HookPosition, TargetObject);

                        if (hit.transform.gameObject.tag == "Enemy")
                        {
                            OnHookEnemyShot?.Invoke();
                        }
                        else
                        {
                            OnHookMapShot?.Invoke();
                        }
                    });
                }
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
                    OnRetractHook?.Invoke(retractSpeed, hook, () =>
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

        #region Gizmos and Debug

#if UNITY_EDITOR
        [System.Serializable]
        public struct GizmoOptions
        {
            public bool grappleRange;
            public bool arrivalDistance;
        }

        [Header("GIZMOS/DEBUG")]
        public bool showGizmos = true;
        public GizmoOptions gizmoOptions = new GizmoOptions()
        {
            grappleRange = false,
            arrivalDistance = true,
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
                Handles.DrawSolidDisc(HookOrigin, Vector3.forward, maxHookDistance);
            }
        }
#endif

        #endregion
    }
}