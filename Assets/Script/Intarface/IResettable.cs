namespace WannaBe
{
    public interface IResettable
    {
        /// <summary>
        /// Скидання стану об'єкта перед поверненням до пулу.
        /// </summary>
        void ResetState();
    }
}
