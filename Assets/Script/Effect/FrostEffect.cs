using UnityEngine;

namespace WannaBe
{
    public class FrostEffect : IEffect, IMovementModifierEffect
    {
        private float _duration;
        public float Duration => _duration;
        public EffectType Type => EffectType.Frost;
        public bool IsExpired => _duration <= 0f;

        // ������� ��������; ���������, 0.7 ������ �������� �������� �� 30%
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
                // ��������� �����, ���� ����� �� ����� ���������
                if (newEffect.Duration > this.Duration)
                    _duration = newEffect.Duration;
                // ���� ����� ����� �������� (������ �������), ����������� ����
                if (newEffect.MovementMultiplier < this.MovementMultiplier)
                    MovementMultiplier = newEffect.MovementMultiplier;
            }
        }

        public void OnApply()
        {
            //Debug.Log("FrostEffect applied.");
            // ��� ����� ��������� �������/������ ������
        }

        public void OnRemove()
        {
            //Debug.Log("FrostEffect removed.");
            // ��������� ������
        }

        public void OnTick(EnemyController enemy, float deltaTime)
        {
            // ��� ������ ���������� ��������� ���� ���������� ���������
            UpdateEffect(deltaTime);
        }
    }
}
