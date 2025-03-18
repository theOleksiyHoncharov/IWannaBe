using UnityEngine;

namespace GPUInstancingRender
{
    /// <summary>
    /// Інтерфейс для об’єктів, які можуть надати дані для GPU‑instancing.
    /// </summary>
    public interface IInstancingData
    {
        /// <summary>
        /// Трансформаційна матриця (позиція, обертання, масштаб) для рендерингу.
        /// </summary>
        Matrix4x4 InstancingMatrix { get; }

        /// <summary>
        /// Унікальний «pivot» для об’єкта (як правило, його позиція).
        /// </summary>
        Vector4 Pivot { get; }
    }
}