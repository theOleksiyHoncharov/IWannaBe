using UnityEngine;

namespace WannaBe
{
    public class FireEffect : IEffect, IDamageMultiplierEffect
    {
        // Приватне поле для зберігання тривалості
        private float _duration;

        public float Duration => _duration;

        public EffectType Type => EffectType.Fire;

        public bool IsExpired => _duration <= 0f;

        public float DamageMultiplier { get; private set; }

        public FireEffect(float duration, float damageMultiplier)
        {
            _duration = duration;
            DamageMultiplier = damageMultiplier;
        }

        public void UpdateEffect(float deltaTime)
        {
            _duration -= deltaTime;
        }

        public void Merge(IEffect other)
        {
            if (other is FireEffect newEffect)
            {
                // Наприклад, подовжуємо ефект, якщо новий має довшу тривалість
                if (newEffect.Duration > this.Duration)
                {
                    _duration = newEffect.Duration;
                }
                // Якщо новий ефект сильніший (більший множник), оновлюємо його
                if (newEffect.DamageMultiplier > this.DamageMultiplier)
                {
                    DamageMultiplier = newEffect.DamageMultiplier;
                }
            }
        }

        public void OnApply()
        {
            Debug.Log("FireEffect застосовано.");
            // Можна додати візуальні ефекти
        }

        public void OnRemove()
        {
            Debug.Log("FireEffect знято.");
            // Прибираємо візуальні ефекти
        }
    }
}
