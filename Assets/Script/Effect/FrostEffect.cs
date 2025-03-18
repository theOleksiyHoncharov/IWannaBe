using UnityEngine;

namespace WannaBe
{
    public class FrostEffect : IEffect, IMovementModifierEffect
    {
        private float _duration;
        public float Duration => _duration;
        public EffectType Type => EffectType.Frost;
        public bool IsExpired => _duration <= 0f;

        // Множник швидкості; наприклад, 0.7 означає зниження швидкості на 30%
        public float MovementMultiplier { get; private set; }

        public FrostEffect(float duration, float movementMultiplier)
        {
            _duration = duration;
            MovementMultiplier = movementMultiplier;
        }

        public void UpdateEffect(float deltaTime)
        {
            _duration -= deltaTime;
        }

        public void Merge(IEffect other)
        {
            if (other is FrostEffect newEffect)
            {
                // Подовжуємо ефект, якщо новий має більшу тривалість
                if (newEffect.Duration > this.Duration)
                    _duration = newEffect.Duration;
                // Якщо новий ефект сильніший (нижчий множник), застосовуємо його
                if (newEffect.MovementMultiplier < this.MovementMultiplier)
                    MovementMultiplier = newEffect.MovementMultiplier;
            }
        }

        public void OnApply()
        {
            //Debug.Log("FrostEffect applied.");
            // Тут можна запустити візуальні/звукові ефекти
        }

        public void OnRemove()
        {
            //Debug.Log("FrostEffect removed.");
            // Прибираємо ефекти
        }

        public void OnTick(EnemyController enemy, float deltaTime)
        {
            // Для ефекту замедлення достатньо лише оновлювати тривалість
            UpdateEffect(deltaTime);
        }
    }
}
