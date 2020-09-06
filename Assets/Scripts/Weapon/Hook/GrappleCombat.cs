using Enemy;
using Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Weapon.Hook
{
    [RequireComponent(typeof(GrapplingGun))]
    public class GrappleCombat : MonoBehaviour
    {
        [SerializeField] PlayerMotor motor = default;
        [SerializeField] EntityHealth playerHp = default;

        /// <summary> The lowest Hit Jump height the player can achieve (which means Combo is 1 or 0) </summary>
        [Tooltip("The lowest Hit Jump height the player can achieve (which means Combo is 0)")]
        public float baseHitJumpHeight = 2f;

        /// <summary> The Hit Jump height multiplier gained from each successful combo </summary>
        [Tooltip("The Hit Jump height multiplier gained from each successful combo")]
        public float hitJumpMultiplier = 1.1f;

        public GameObject slashVFX;

		public UnityEvent onEnemyKilled;

        public int Combo => 1; // TODO: implement Combo system
        private float CurrentHitJumpHeight
        {
            get => baseHitJumpHeight * hitJumpMultiplier * Combo;
        }

        private GrapplingGun grapple;

        private void Awake()
        {
            grapple = GetComponent<GrapplingGun>();
        }

        private void OnEnable()
        {
            GrapplingGun.OnPullEnded += GrapplePullEnded;
            GrapplingGun.OnHookTargetHit += GrapplingHookTargetHit;
        }

        private void OnDisable()
        {
            GrapplingGun.OnPullEnded -= GrapplePullEnded;
            GrapplingGun.OnHookTargetHit -= GrapplingHookTargetHit;
        }

        #region Callbacks

        private void GrapplingHookTargetHit(Vector2 hookPosition,
            Collider2D targetObject)
        {
            // Maybe make an interface for hooked object so they can
            // have behaviour for what happens after they got hooked,
            // ex: A moving flying enemy might stops after they got hooked, etc
            // Should also add hook released event if this is implemented

            // Example code:
            // IHookable targetHookable = targetObject.GetComponent<IHookable>();
            // if (targetHookable != null) targetHookable.GetHooked(hookPosition);
        }

        private void GrapplePullEnded(bool arrivedAtTarget, Collider2D targetObject,
            Collider2D collidedObject)
        {
            // If the grapple stops not within the grapples reach(GrapplingGun.arrivedDistance)
            if (!arrivedAtTarget)
            {
                // If we somehow doesn't hit anything, just don't do anything
                if (collidedObject == null) return;

                //Debug.LogFormat("The player failed to arrive at the target and collided with {0}.", collidedObject.name);

                // If we didn't hit an enemy, don't do anything
                EnemyController collidedEnemy = GetEnemyFromCollider(collidedObject);
                if (collidedEnemy == null) return;
                else
                {
                    // If we hit an enemy, check if the enemy is the target
                    // if it is then hit them, else let the player take damage
                    // see CollideWithEnemy method for more detail
                    CollideWithEnemy(collidedEnemy, collidedObject == targetObject);
                }
            }
            // If we arrive at the target successfully (stops within the grapples reach)
            else
            {
                //Debug.LogFormat("The player arrives at the target named {0}.", targetObject.name);

                // Check if the target is an enemy or not
                // if not don't do anything
                EnemyController targetEnemy = GetEnemyFromCollider(targetObject);
                if (targetEnemy == null) return;

                // if it is, then hit the enemy
                CollideWithEnemy(targetEnemy, true);
            }
        }

        #endregion

        private EnemyController GetEnemyFromCollider(Collider2D collider,
            bool checkOtherColliderOnRigid = false)
        {
            EnemyController enemy = collider.GetComponent<EnemyController>();
            //Debug.Log(collider);
            if (enemy == null && collider.attachedRigidbody != null)
            {
                enemy = collider.attachedRigidbody.GetComponent<EnemyController>();
                if (enemy == null && checkOtherColliderOnRigid)
                {
                    List<Collider2D> colliders = new List<Collider2D>();
                    collider.attachedRigidbody.GetAttachedColliders(colliders);

                    foreach (Collider2D c in colliders)
                    {
                        enemy = c.GetComponent<EnemyController>();
                        if (enemy != null) break;
                    }
                }
            }

            return enemy;
        }

        private void CollideWithEnemy(EnemyController enemy, bool hitEnemy)
        {
            if (hitEnemy)
            {
                // Maybe have an IDamageable interface so we can control what enemy should do
                // when they take the players damage?

                enemy.Damage(1);
                InstantiateVFX(enemy.gameObject);
                //StartCoroutine(motor.GetComponent<EntityHealth>().MakeInvincible(1f));
                if (enemy.CurrentHealth <= 0)
                {
                    // Destroy(enemy.gameObject);
                    PerformJumpHit();
                    motor.GetComponent<PlayerHealth>().IncreaseHealth(1);

					onEnemyKilled?.Invoke();
				}
            }

            // The interfaces should be able to make us able to pass any Entity
            // rather than passing the EnemyController to the CollideFunction
            //
            // Example Code: CollideWith(Entity collidedEntity)
            //IDamaging entityDamaging = (IDamaging)collidedEntity;
            //if (collidedEntity)
            //{
            //    entityDamaging.Damage(player);
            //    return;
            //}
            //
            //IDamageable entityDamageable = (IDamageable)collidedEntity;
            //if (collidedEntity)
            //{
            //    entityDamageable.Damage(player);
            //    return;
            //}
            //
            // Or we could have one ICollideable interface to manage it all
        }

        private void InstantiateVFX(GameObject enemy)
        {
            Vector3 difference = transform.position - enemy.transform.position;
            bool facesRight = motor.GetComponent<PlayerController>().FacesRight();
            bool facesTop = difference.y <= 0;
            SpriteRenderer slashRenderer = Instantiate(slashVFX, motor.transform.position, Quaternion.Euler(Vector3.zero)).GetComponent<SpriteRenderer>(); ;

            if (Mathf.Abs(difference.x) > Mathf.Abs(difference.y))
            {
                if (facesRight)
                {
                    slashRenderer.flipX = false;
                }
                else
                {
                    slashRenderer.flipX = true;
                }
            }
            else
            {
                if (facesTop)
                {
                    slashRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                }
                else
                {
                    slashRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
                }
            }

        }


        // HACK: Could someone implement a jump hit method in the PlayerMotor class
        // Ideally it would take a jump height for the argument so i can just send
        // the optimal jump height based on the combo, etc
        private void PerformJumpHit()
        {
            Vector2 targetVelocity = Vector2.zero;
            targetVelocity.y = CurrentHitJumpHeight;
            targetVelocity.y =
                Mathf.Sqrt(2f * motor.FallGravity * CurrentHitJumpHeight);

            Vector2 impulse = targetVelocity - motor.Body.velocity;
            motor.Body.AddForce(impulse * motor.Body.mass, ForceMode2D.Impulse);
        }
    }
}