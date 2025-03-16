using UnityEngine;

namespace WannaBe
{
    public interface IFinishPointProvider
    {
        /// <summary>
        /// Повертає позицію, яка є ціллю для ворогів.
        /// </summary>
        Vector3 GetFinishPoint();
    }
}