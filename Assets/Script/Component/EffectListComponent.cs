using UnityEngine;

namespace WannaBe
{
    public class EffectListComponent : MonoBehaviour
    {
        public EffectList Effects { get; private set; } = new EffectList();

        private void Update()
        {
            Effects.UpdateEffects(Time.deltaTime);
        }

        /// <summary>
        /// Викликається при скиданні ворога (наприклад, поверненні в пул).
        /// </summary>
        public void ResetEffects()
        {
            Effects.ResetEffects();
        }
    }
}
