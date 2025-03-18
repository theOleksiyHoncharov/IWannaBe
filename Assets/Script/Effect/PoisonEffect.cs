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

        // ������ �� DPS ��� Merge-���������
        public float DamagePerSecond => _damagePerSecond;

        public PoisonEffect(float duration, float damagePerSecond)
        {
            _duration = duration;
            _damagePerSecond = damagePerSecond;
        }

        public void UpdateEffect(float deltaTime)
        {
            _duration -= deltaTime;
        }

        public void Merge(IEffect other)
        {
            if (other is PoisonEffect newEffect)
            {
                // ��������� �����, ���� ����� �� ����� ���������
                if (newEffect.Duration > this.Duration)
                    _duration = newEffect.Duration;
                // ���� ����� ����� �� ������ DPS, ����������� ����
                if (newEffect.DamagePerSecond > this.DamagePerSecond)
                    _damagePerSecond = newEffect.DamagePerSecond;
            }
        }

        public void OnApply()
        {
            Debug.Log("PoisonEffect applied.");
            // �������� ������� ������, ���� �������
        }

        public void OnRemove()
        {
            Debug.Log("PoisonEffect removed.");
            // �������� ������� ������
        }

        public void OnTick(EnemyController enemy, float deltaTime)
        {
            _tickTimer += deltaTime;
            // �������� ����������� ��� �� �������
            if (_tickTimer >= 1f)
            {
                int ticks = Mathf.FloorToInt(_tickTimer);
                enemy.TakeDamage(_damagePerSecond * ticks);
                _tickTimer -= ticks;
            }
            UpdateEffect(deltaTime);
        }
    }
}
