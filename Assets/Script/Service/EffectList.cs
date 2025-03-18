using System.Collections.Generic;

namespace WannaBe
{
    public class EffectList
    {
        private readonly List<IEffect> _effects = new List<IEffect>();

        public void AddEffect(IEffect newEffect)
        {
            var existing = _effects.Find(e => e.Type == newEffect.Type);
            if (existing != null)
            {
                existing.Merge(newEffect);
            }
            else
            {
                _effects.Add(newEffect);
                newEffect.OnApply();
            }
        }

        public void UpdateEffects(float deltaTime)
        {
            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                _effects[i].UpdateEffect(deltaTime);
                if (_effects[i].IsExpired)
                {
                    _effects[i].OnRemove();
                    _effects.RemoveAt(i);
                }
            }
        }

        public float GetDamageMultiplier()
        {
            float multiplier = 1f;
            foreach (var effect in _effects)
            {
                if (effect is IDamageMultiplierEffect dmgEffect)
                {
                    multiplier *= dmgEffect.DamageMultiplier;
                }
            }
            return multiplier;
        }

        public float GetMovementMultiplier()
        {
            float multiplier = 1f;
            foreach (var effect in _effects)
            {
                if (effect is IMovementModifierEffect moveEffect)
                {
                    multiplier *= moveEffect.MovementMultiplier;
                }
            }
            return multiplier;
        }

        /// <summary>
        /// Повністю очищує всі ефекти (наприклад, при скиданні ворога в пул).
        /// </summary>
        public void ResetEffects()
        {
            foreach (var effect in _effects)
            {
                effect.OnRemove();
            }
            _effects.Clear();
        }
    }
}
