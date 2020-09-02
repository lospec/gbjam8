using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

class EntityHealth : MonoBehaviour
{
    [SerializeField] private int health;
    public int Health
    {
        set => health = value;
        get => health;
    }

    public UnityEvent<int> OnTakeDamage;

    public virtual void Hurt(int damage)
    {
        Health -= damage;
        OnTakeDamage?.Invoke(damage);
    }
}