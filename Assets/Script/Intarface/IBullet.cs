using UnityEngine;

namespace WannaBe
{
    public interface IBullet
    {
        /// <summary>
        /// Ініціалізує кулю, задаючи напрямок руху, який обчислюється як різниця між позицією цілі та стартовою позицією.
        /// Також встановлює орієнтацію кулі відповідно до напрямку.
        /// </summary>
        /// <param name="startPosition">Початкова позиція кулі.</param>
        /// <param name="enemyTransform">Transform ворога, до якого летить куля.</param>
        void Initialize(Vector3 startPosition, Transform enemyTransform);
    }
}