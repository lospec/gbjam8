public interface ITakeDamage
{
    void Damage(float damage);
    float CurrentHealth { get; }
}