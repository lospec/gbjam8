using System.Collections;
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
                KillEnemy ke = GetComponent<KillEnemy>();

                if (ke != null)
                {
                    ke.Explode();
                }
            }
        }
    }

    public IEnumerator MakeInvincible(float time)
    {
        canTakeDamage = false;

        yield return new WaitForSeconds(time);

        canTakeDamage = true;
    }
}