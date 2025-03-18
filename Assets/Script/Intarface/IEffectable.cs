namespace WannaBe
{
    public interface IEffectable
    {
        /// <summary>
        /// Накладає новий ефект або оновлює вже існуючий.
        /// </summary>
        void ApplyEffect(IEffect effect);

        /// <summary>
        /// Повертає множник пошкоджень з ефектів.
        /// </summary>
        float GetDamageMultiplier();

        /// <summary>
        /// Повертає множник швидкості з ефектів.
        /// </summary>
        float GetMovementMultiplier();

        /// <summary>
        /// Скидає всі активні ефекти (наприклад, при поверненні в пул).
        /// </summary>
        void ResetEffects();
    }
}
