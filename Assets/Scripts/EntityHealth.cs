﻿using UnityEngine;
using UnityEngine.Events;

class EntityHealth : MonoBehaviour
{
	[SerializeField] private int _health;
	public int Health
	{
		protected set
		{
			_health = value;
			OnHealthSet?.Invoke(value);
		}
		get
		{
			return _health;
		}
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