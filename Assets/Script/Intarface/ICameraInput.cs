using UnityEngine;
namespace WannaBe
{
    public interface ICameraInput
    {
        /// <summary>
        /// Повертає значення правого стіку (Vector2).
        /// </summary>
        Vector2 GetRightStickValue();
    }
}