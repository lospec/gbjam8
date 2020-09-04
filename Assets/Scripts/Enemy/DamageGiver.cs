using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageGiver : MonoBehaviour
{
    public int damage;
    public float knockBackStrength;
    public float knockBackDuration;
    public float invincibilityTime;

    private bool collided = false;

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
        if (collision.tag.Contains("Player") && !collided)
        {
            Player.PlayerController player = collision.GetComponent<Player.PlayerController>();
            EntityHealth health = player.GetComponent<EntityHealth>();

            if (!player.isKnockingBack && health.canTakeDamage)
            {
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
}
