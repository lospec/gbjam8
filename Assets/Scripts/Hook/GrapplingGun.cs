using System;
using System.Collections;
using Player;
using UnityEngine;
using Weapon;

namespace Hook
{
    public class GrapplingGun : MonoBehaviour
    {
        /// <summary> The speed of the hook when shot, use 0 for instant shot </summary>
        [Tooltip("The speed of the hook when shot, use 0 for instant shot")]
        public float shootSpeed = 0f;

        /// <summary> The speed of the hook when shot, use 0 for instant shot </summary>
        [Tooltip("The grapples pulling speed")]
        public float pullSpeed = 50f;

        /// <summary> The maximum range the player can grapple, use 0 for infinite range </summary>
        [Tooltip("The maximum range the player can grapple, use 0 for infinite range")]
        public float maxHookDistance = 0f;

        /// <summary>
        ///     The time it takes in seconds for the hook to return if it can't hit a target, this will only be used when
        ///     Shoot Speed and Max Hook Distance Speed is more than 0 (and not Infinity for the distance)
        /// </summary>
        [Tooltip(
            "The time it takes in seconds for the hook to return if it can't hit a target, this will only be used when Shoot Speed and Max Hook Distance Speed is more than 0 (and not Infinity for the distance)")]
        public float hookReturnDuration = 1f;
        [SerializeField] private Transform hook;
        [SerializeField] private Rope rope;


        private Vector2 _aim = Vector2.right;

        private float lastDirection = 1f;

        private Coroutine shootFalseHook = null;
        public bool ShowHook => enabled || shootFalseHook != null;
        public Vector2 HookOrigin => Motor.Body.position;

        public Vector2 HookPosition
        {
            get => hook.transform.position;
            set => hook.transform.position = value;
        }
        public Vector2 HookDirection { get; private set; }
        public Vector2 Target { get; private set; }

        public PlayerMotor Motor { get; set; }

        public Vector2 Aim
        {
            get => _aim;
            set => _aim = new Vector2
            {
                x = Mathf.Abs(value.x) > 0f
                    ? new Func<float>(() =>
                    {
                        lastDirection = Mathf.Sign(value.x);
                        return value.x;
                    }).Invoke()
                    : Mathf.Abs(value.y) > 0f
                        ? 0
                        : lastDirection,
                y = value.y
            };
        }

        private void FixedUpdate()
        {
            if ((Target - HookPosition).sqrMagnitude > 0.01f)
            {
                // HookPosition = Vector2.MoveTowards(HookPosition, Target,
                //     shootSpeed * Time.fixedDeltaTime);
                return;
            }

            if (Motor.enabled)
                DisableMovement();

            MoveTowardsTarget();
        }

        public void MoveTowardsTarget()
        {
            // if anybody have a better way to move the player, feel free to change my script
            var dir = (Target - HookOrigin).normalized;
            var vel = dir * pullSpeed;

            // Player hits an obstacle
            var hits = new RaycastHit2D[4];
            if (Motor.Body.Cast(dir, hits, pullSpeed * Time.fixedDeltaTime) > 0f)
                // Could additionally check it the obstacle stops grappling or not, but this'll do for now
                StopGrappling();

            Motor.Body.AddForce(vel, ForceMode2D.Impulse);

            // Player reaches the target
            if (Motor.Body.OverlapPoint(Target))
                StopGrappling();
        }

        public void StopGrappling()
        {
            enabled = false;
            EnableMovement();

            // Should add other functionalities here
            // Maybe check if the target is an enemy and kill it, check for jump, manage combo, etc
            // or Maybe add OnGrappleStop event to this script?
        }

        public void PerformGrapple()
        {
            if (shootFalseHook != null)
            {
                StopCoroutine(shootFalseHook);
                shootFalseHook = null;
            }

            var hit = maxHookDistance > 0f
                ? Physics2D.Raycast(HookOrigin, _aim, maxHookDistance)
                : Physics2D.Raycast(HookOrigin, _aim);

            if (hit.collider)
            {
                var hitObject = hit.collider.gameObject;

                // i don't know what we will use to determine if the object is hookable, so i comment out the if statement for now
                // ideally we want to check what the object is and grapple accordingly
                // if it's an enemy do GrappleEnemy, if it's a wall do GrappleWall, etc

                //if (hitObject.CompareTag("Tile"))
                enabled = true;
                Target = hit.point;
                HookDirection = _aim;
                HookPosition = shootSpeed > 0f ? HookOrigin : Target;
                rope.StartConnect();
                hook.SetParent(null, true);
            }

            else if (shootSpeed > 0f && maxHookDistance > 0f &&
                     !float.IsPositiveInfinity(maxHookDistance))
            {
                Target = HookOrigin + _aim.normalized * maxHookDistance;
                HookPosition = HookOrigin;
                HookDirection = _aim;
                rope.StartConnect();
                hook.SetParent(null, true);
                shootFalseHook = StartCoroutine(ShootFalseHook());
            }
        }

        private IEnumerator ShootFalseHook()
        {
            while ((Target - HookPosition).sqrMagnitude > 0.01f)
            {
                // HookPosition = Vector2.MoveTowards(HookPosition, Target,
                //     shootSpeed * Time.deltaTime);
                yield return null;
            }

            if (hookReturnDuration <= 0f)
            {
                shootFalseHook = null;
                yield break;
            }

            var t = 0f;
            while (t < 1f)
            {
                // We could still use this, but i'm afraid that this will behave strangely
                // because the HookOrigin could move Away from the hook position
                //HookPosition = Vector2.MoveTowards(HookPosition, HookOrigin, shootSpeed * Time.deltaTime);
                t += Time.deltaTime / hookReturnDuration;
                HookPosition = Vector2.Lerp(Target, HookOrigin, t);
                yield return null;
            }

            rope.Line.enabled = false;
            shootFalseHook = null;
            hook.SetParent(transform, true);
        }

        private void DisableMovement()
        {
            Motor.Body.velocity = Vector2.zero;
            Motor.enabled = false;
        }

        private void EnableMovement()
        {
            rope.Line.enabled = false;
            hook.SetParent(transform, true);
            Motor.Body.velocity = Vector2.zero;
            Motor.enabled = true;
        }
    }
}