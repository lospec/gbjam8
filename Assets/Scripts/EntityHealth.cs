using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Utility;

class EntityHealth : MonoBehaviour
{
    [SerializeField] private int health;
    public bool canTakeDamage = true;
    public int Health
    {
        set
		{
			health = value;
			OnHealthSet?.Invoke(value);
		}
        get => health;
    }

	public UnityEvent<int> OnHealthSet;
	public UnityEvent<int> OnTakeDamage;

	private void Start()
	{
		Health = health;
	}

    public virtual void Hurt(int damage)
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