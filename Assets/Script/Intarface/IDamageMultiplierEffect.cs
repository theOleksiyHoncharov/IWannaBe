namespace WannaBe
{
    public interface IDamageMultiplierEffect
    {
        /// <summary>
        /// Множник, який застосовується до базового дамагу.
        /// Наприклад, 1.5 означає +50% отримуваного пошкодження.
        /// </summary>
        float DamageMultiplier { get; }
    }
}