using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inputs;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Rendering;

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

        /// <summary> The distance that the character will cast to check if they collided with walls or not </summary>
        [Tooltip("The distance that the character will cast to check if they collided with walls or not")]
        public float checkDistance = .25f;

        public bool IsGrappling { get; private set; } = false;
        public Vector2 HookPosition { get; private set; }
        public Vector2 HookDirection { get; private set; }
        public Vector2 Aim { get; private set; }
        public Vector2 Target { get; private set; }

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

            if (input.sqrMagnitude >= 1f)
                Aim = input;
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
            Vector2 dir = (Target - rigid.position).normalized;
            Vector2 vel = dir * pullSpeed;

            // Player hits an obstacle
            // Don't know why this doesn't work here
            RaycastHit2D[] hits = new RaycastHit2D[4];
            if (rigid.Cast(dir, hits, pullSpeed * Time.deltaTime) > 0)
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
            if (IsGrappling) return;

            RaycastHit2D hit;
            if (maxHookDistance > 0)
                hit = Physics2D.Raycast(rigid.position, Aim, maxHookDistance);
            else
                hit = Physics2D.Raycast(rigid.position, Aim);

            if (hit)
            {
                GameObject hitObject = hit.collider.gameObject;

                // i don't know what we will use to determine if the object is hookable, so i comment out the if statement for now
                // ideally we want to check what the object is and grapple accordingly
                // if it's an enemy do GrappleEnemy, if it's a wall do GrappleWall, etc

                //if (hitObject.CompareTag("Tile"))
                {
                    // if anybody have a better way to move the player, feel free to change my script
                    IsGrappling = true;
                    Target = hit.point;
                    HookDirection = Aim;

                    if (shootSpeed > 0f)
                        HookPosition = rigid.position;
                    else
                        HookPosition = Target;
                }
            }
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
