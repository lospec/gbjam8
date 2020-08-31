using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inputs;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Rendering;
using System.Data.Common;

namespace Hook.Prototype
{
    public class PlayerHookA : MonoBehaviour
    {
        [SerializeField] MonoBehaviour playerMovementComponent = default;

        /// <summary> The speed of the hook when shot, use 0 for instant shot </summary>
        [Tooltip("The speed of the hook when shot, use 0 for instant shot")]
        public float shootSpeed = 0f;
        
        /// <summary> The speed of the hook when shot, use 0 for instant shot </summary>
        [Tooltip("The grapples pulling speed")]
        public float pullSpeed = 50f;

        /// <summary> The maximum range the player can grapple, use 0 for infinite range </summary>
        [Tooltip("The maximum range the player can grapple, use 0 for infinite range")]
        public float maxHookDistance = 0f;

        /// <summary> The time it takes in seconds for the hook to return if it can't hit a target, this will only be used when Shoot Speed and Max Hook Distance Speed is more than 0 (and not Infinity for the distance) </summary>
        [Tooltip("The time it takes in seconds for the hook to return if it can't hit a target, this will only be used when Shoot Speed and Max Hook Distance Speed is more than 0 (and not Infinity for the distance)")]
        public float hookReturnDuration = 1f;

        public bool IsGrappling { get; private set; } = false;
        public bool ShowHook { get => IsGrappling || shootFalseHook != null; }
        public Vector2 HookOrigin { get => rigid.position; }
        public Vector2 HookPosition { get; private set; }
        public Vector2 HookDirection { get; private set; }
        public Vector2 Aim { get; private set; } = Vector2.right;
        public Vector2 Target { get; private set; }

        private float lastDirection = 1f;
        private Rigidbody2D rigid;
        private PlayerControls input;
        
        #region Setups

        private void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            input = new PlayerControls();
        }

        private void Start()
        {
            input.Player.Movement.performed += MovementPerformed;
            input.Player.Movement.canceled += MovementCanceled;
            input.Player.Secondary.performed += SecondaryPerformed;
        }

        private void OnEnable()
        {
            input.Enable();
        }

        private void OnDisable()
        {
            input.Disable();
        }

        #endregion

        #region Input Callbacks

        private void MovementPerformed(InputAction.CallbackContext ctx)
        {
            Vector2 input = ctx.ReadValue<Vector2>();

            if (input.sqrMagnitude >= 1f) Aim = input;
            else Aim = Vector2.right * lastDirection;

            if (Mathf.Abs(input.x) > 0f)
                lastDirection = Mathf.Sign(input.x);
        }

        private void MovementCanceled(InputAction.CallbackContext ctx)
        {
            Aim = Vector2.right * lastDirection;
        }

        private void SecondaryPerformed(InputAction.CallbackContext ctx)
        {
            PerformGrapple();
        }

        #endregion

        private void FixedUpdate()
        {
            if (!IsGrappling) return;

            if ((Target - HookPosition).sqrMagnitude > 0.01f)
            {
                HookPosition = Vector2.MoveTowards(HookPosition, Target, shootSpeed * Time.deltaTime);
                return;
            }

            if (playerMovementComponent.enabled)
                DisableMovement();

            MoveTowardsTarget();
        }

        public void MoveTowardsTarget()
        {
            // if anybody have a better way to move the player, feel free to change my script
            Vector2 dir = (Target - HookOrigin).normalized;
            Vector2 vel = dir * pullSpeed;

            // Player hits an obstacle
            RaycastHit2D[] hits = new RaycastHit2D[4];
            if (rigid.Cast(dir, hits, pullSpeed * Time.deltaTime) > 0f)
            {
                // Could additionally check it the obstacle stops grappling or not, but this'll do for now
                StopGrappling();
            }

            rigid.MovePosition(rigid.position + vel * Time.deltaTime);

            // Player reaches the target
            if (rigid.OverlapPoint(Target))
                StopGrappling();
        }

        public void StopGrappling()
        {
            IsGrappling = false;
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

            RaycastHit2D hit;

            if (maxHookDistance > 0f)
                hit = Physics2D.Raycast(HookOrigin, Aim, maxHookDistance);
            else
                hit = Physics2D.Raycast(HookOrigin, Aim);

            if (hit.collider)
            {
                GameObject hitObject = hit.collider.gameObject;

                // i don't know what we will use to determine if the object is hookable, so i comment out the if statement for now
                // ideally we want to check what the object is and grapple accordingly
                // if it's an enemy do GrappleEnemy, if it's a wall do GrappleWall, etc

                //if (hitObject.CompareTag("Tile"))
                {
                    IsGrappling = true;
                    Target = hit.point;
                    HookDirection = Aim;

                    if (shootSpeed > 0f)
                        HookPosition = HookOrigin;
                    else
                        HookPosition = Target;
                }
            }

            else if (shootSpeed > 0f && maxHookDistance > 0f && maxHookDistance != Mathf.Infinity)
            {
                Target = HookOrigin + Aim.normalized * maxHookDistance;
                HookPosition = HookOrigin;
                HookDirection = Aim;

                shootFalseHook = StartCoroutine(ShootFalseHook());
            }
        }

        private Coroutine shootFalseHook = null;
        private IEnumerator ShootFalseHook()
        {
            while ((Target - HookPosition).sqrMagnitude > 0.01f)
            {
                Debug.Log("ASKDLASD");
                HookPosition = Vector2.MoveTowards(HookPosition, Target, shootSpeed * Time.deltaTime);
                yield return null;
            }

            if (hookReturnDuration <= 0f)
            {
                shootFalseHook = null;
                yield break;
            }

            float t = 0f;
            while (t < 1f)
            {
                // We could still use this, but i'm afraid that this will behave strangely
                // because the HookOrigin could move Away from the hook position
                //HookPosition = Vector2.MoveTowards(HookPosition, HookOrigin, shootSpeed * Time.deltaTime);
                t += Time.deltaTime / hookReturnDuration;
                HookPosition = Vector2.Lerp(Target, HookOrigin, t);
                yield return null;
            }

            shootFalseHook = null;
        }

        private void DisableMovement()
        {
            rigid.isKinematic = true;
            rigid.velocity = Vector2.zero;
            playerMovementComponent.enabled = false;
        }

        private void EnableMovement()
        {
            rigid.isKinematic = false;
            rigid.velocity = Vector2.zero;
            playerMovementComponent.enabled = true;
        }
    }
}
