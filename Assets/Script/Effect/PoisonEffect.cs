using UnityEngine;

namespace WannaBe
{
    public class PoisonEffect : IEffect
    {
        private float _duration;
        private float _damagePerSecond;
        private float _tickTimer = 0f;

        public float Duration => _duration;
        public EffectType Type => EffectType.Poison;
        public bool IsExpired => _duration <= 0f;

        // Доступ до DPS для Merge-порівняння
        public float DamagePerSecond => _damagePerSecond;

        IDamageable _damageableTarget;
        IEffectable _effectableTarget;

        public PoisonEffect(float duration, float damagePerSecond, IDamageable damageable, IEffectable effectable)
        {
            _duration = duration;
            _damagePerSecond = damagePerSecond;
            _damageableTarget = damageable;
            _effectableTarget = effectable;
        }

        public void UpdateEffect(float deltaTime)
        {
            OnTick(deltaTime);
            _duration -= deltaTime;
        }

        public void Merge(IEffect other)
        {
            if (other is PoisonEffect newEffect)
            {
                // Подовжуємо ефект, якщо новий має більшу тривалість
                if (newEffect.Duration > this.Duration)
                    _duration = newEffect.Duration;
                // Якщо новий ефект має більший DPS, застосовуємо його
                if (newEffect.DamagePerSecond > this.DamagePerSecond)
                    _damagePerSecond = newEffect.DamagePerSecond;
            }
        }

        public void OnApply()
        {
            Debug.Log("PoisonEffect applied.");
            // Запустіть візуальні ефекти, якщо потрібно
        }

        public void OnRemove()
        {
            Debug.Log("PoisonEffect removed.");
            // Приберіть візуальні ефекти
        }

        public void OnTick(float deltaTime)
        {
            _tickTimer += deltaTime;
            // Наносимо пошкодження раз на секунду
            if (_tickTimer >= 1f)
            {
                int ticks = Mathf.FloorToInt(_tickTimer);
                _damageableTarget.TakeDamage(_damagePerSecond * ticks);
                _tickTimer -= ticks;
            }
        }
    }
}
