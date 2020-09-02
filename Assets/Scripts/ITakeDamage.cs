public interface ITakeDamage
{
    void Damage(int damage);
    int CurrentHealth { get; }
}