using UnityEngine;
using UnityEngine.Events;

class EntityHealth : MonoBehaviour
{
    [SerializeField] private int health;
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
        Health -= damage;
        OnTakeDamage?.Invoke(damage);
    }
}