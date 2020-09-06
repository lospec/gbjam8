using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utility;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private float health;
    public bool canTakeDamage = true;
    public float Health
    {
        set
		{
			health = value;
			OnHealthSet?.Invoke(value);
		}
        get => health;
    }

	public UnityEvent<float> OnHealthSet;
	public UnityEvent<float> OnTakeDamage;

	private void Start()
	{
		Health = health;
	}

    public virtual void Hurt(float damage)
    {
        if (canTakeDamage)
        {
            Health -= damage;
            OnTakeDamage?.Invoke(damage);

            if (Health <= 0)
            {
                // HACK: Please make a more centralized system to destroy the enemy, if you want to seperate the scripts, just make use of events and don't make dependencies EVERYWHERE
                List<Collider2D> attachedColliders = new List<Collider2D>();
                GetComponent<Rigidbody2D>().GetAttachedColliders(attachedColliders);
                foreach (Collider2D c in attachedColliders)
                    c.enabled = false;

				Die();
            }
        }
    }

    public IEnumerator MakeInvincible(float time)
    {
        canTakeDamage = false;

        yield return new WaitForSeconds(time);

        canTakeDamage = true;
    }

	public virtual void Die()
	{
		KillEnemy ke = GetComponent<KillEnemy>();

		if (ke != null)
		{
			ke.Explode();
		}
	}
}