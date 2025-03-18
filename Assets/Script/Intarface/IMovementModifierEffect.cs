namespace WannaBe
{
    public interface IMovementModifierEffect
    {
        /// <summary>
        /// Множник, який застосовується до базової швидкості.
        /// Наприклад, 0.8 означає зниження швидкості на 20%.
        /// </summary>
        float MovementMultiplier { get; }
    }
}