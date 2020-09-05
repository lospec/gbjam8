using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
		OnHealthSet?.Invoke(Health);
	}

	public virtual void Hurt(int damage)
    {
        if (canTakeDamage)
        {
            Health -= damage;
            OnTakeDamage?.Invoke(damage);
        }
    }

    public IEnumerator MakeInvincible(float time)
    {
        canTakeDamage = false;

        yield return new WaitForSeconds(time);

        canTakeDamage = true;
    }
}