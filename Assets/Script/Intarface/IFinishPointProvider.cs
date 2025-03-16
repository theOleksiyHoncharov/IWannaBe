namespace WannaBe
{
    public interface IFinishPointProvider
    {
        /// <summary>
        /// Повертає трансформ, який є ціллю для ворогів.
        /// </summary>
        UnityEngine.Transform GetFinishPoint();
    }
}