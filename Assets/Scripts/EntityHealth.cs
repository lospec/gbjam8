using UnityEngine;

class EntityHealth : MonoBehaviour
{
	[SerializeField] private int _health;
	public int Health
	{
		protected set
		{
			_health = value;
			OnHealthSet(value);
		}
		get
		{
			return _health;
		}
	}

	public delegate void HealthSetEventHandler(int health);
	public event HealthSetEventHandler HealthSetEvent;

	public delegate void TakeDamageEventHandler(int damage);
	public event TakeDamageEventHandler TakeDamageEvent;

	protected virtual void OnHealthSet(int health)
	{
		HealthSetEvent?.Invoke(health);
	}

	protected virtual void OnTakeDamage(int damage)
	{
		TakeDamageEvent?.Invoke(damage);
	}

	private void Start()
	{
		OnHealthSet(Health);
	}

	public virtual void TakeDamage(int damage)
	{
		Health -= damage;
		OnTakeDamage(damage);
	}
}
