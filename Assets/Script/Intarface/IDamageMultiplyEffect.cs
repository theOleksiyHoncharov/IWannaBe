public interface IDamageMultiplyEffect
{
    /// <summary>
    /// Множник, який застосовується до базового дамагу.
    /// Наприклад, 1.5 означає +50% отримуваного дамагу.
    /// </summary>
    float DamageMultiplier { get; }
}