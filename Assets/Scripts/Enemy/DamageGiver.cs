using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;
using Weapon.Hook;

public class DamageGiver : MonoBehaviour
{
    public int damage;
    public float knockBackStrength;
    public float knockBackDuration;
    public float invincibilityTime;

    private bool collided = false;
    private GrapplingGun grapple;

    private void Start()
    {
        grapple = GameManager.instance.playerObject.GetComponentInChildren<GrapplingGun>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollisionManagement(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        CollisionManagement(collision);
    }

    private void CollisionManagement(Collider2D collision)
    {
        if (collision.tag.Contains("Player") && !collided && !IsTarget())
        {
            Player.PlayerController player = collision.GetComponent<Player.PlayerController>();
            EntityHealth health = player.GetComponent<EntityHealth>();

            if (!player.isKnockingBack && health.canTakeDamage)
            {
                Debug.Log("Damaging player");

                collided = true;
                health.Hurt(damage);

                StartCoroutine(collision.GetComponent<Player.PlayerController>().Knockback(
                    knockBackDuration, knockBackStrength, transform.position.x < collision.transform.position.x,
                    invincibilityTime));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag.Contains("Player"))
        {
            collided = false;
        }
    }

    private bool IsTarget()
    {
        Collider2D grappleTarget = grapple.TargetObject;
        Collider2D[] thisColliders = GetComponentsInChildren<Collider2D>();

        for (int i = 0; i < thisColliders.Length; i++)
        {
            if (thisColliders[i].Equals(grappleTarget))
            {
                return true;
            }
        }

        return false;
    }
}
