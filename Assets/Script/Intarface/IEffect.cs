namespace WannaBe
{
    public interface IEffect
    {
        /// <summary>
        /// Тривалість ефекту (тільки для читання ззовні)
        /// </summary>
        float Duration { get; }

        /// <summary>
        /// Тип ефекту (для порівняння, чи ефект вже накладено)
        /// </summary>
        EffectType Type { get; }

        /// <summary>
        /// Метод, який зменшує час дії ефекту. 
        /// Викликається кожного тіка.
        /// </summary>
        void UpdateEffect(float deltaTime);

        /// <summary>
        /// Повертає, чи завершився ефект
        /// </summary>
        bool IsExpired { get; }

        /// <summary>
        /// Об'єднує новий ефект із вже накладеним (наприклад, подовжує Duration або замінює параметри)
        /// </summary>
        /// <param name="other">Новий ефект того самого типу</param>
        void Merge(IEffect other);

        /// <summary>
        /// Викликається, коли ефект застосовано
        /// </summary>
        void OnApply();

        /// <summary>
        /// Викликається, коли ефект знімається
        /// </summary>
        void OnRemove();
    }
}
