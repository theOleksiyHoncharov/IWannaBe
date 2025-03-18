using UnityEngine;
using Zenject;

namespace WannaBe
{
    public class PoisonBullet : BaseBullet
    {
        public override BulletType BulletType => BulletType.Poison;

        [Header("������������ Poison Bullet")]
        [SerializeField] private float initialSpeed = 14f;
        [SerializeField] private float damage = 15f;
        [SerializeField] private float hitThreshold = 0.5f;
        [SerializeField] private float poisonEffectDuration = 6f;
        [SerializeField] private float poisonEffectDps = 5f; // damage per second

        private Transform target;

        private Vector3 _predictedPosition;
        public override void Initialize(Vector3 startPosition, Transform enemyTransform)
        {
            transform.rotation = Quaternion.identity;
            target = enemyTransform;

            // �������� �������� ��� ����� ITargetMotion (���� ����������)
            Vector3 targetVelocity = Vector3.zero;
            var targetMotion = enemyTransform.GetComponent<ITargetMotion>();
            if (targetMotion != null)
            {
                targetVelocity = targetMotion.Velocity;
            }

            // ���������� ������� �� �������� ����� �� ������� ������� ������
            float distance = Vector3.Distance(startPosition, enemyTransform.position);
            // ���������� ��� ������� ���
            float flightTime = distance / initialSpeed;
            // ������������ ������� ������
            _predictedPosition = enemyTransform.position + targetVelocity * flightTime;
            transform.position = startPosition;
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, _predictedPosition, initialSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _predictedPosition) <= hitThreshold)
            {
                if (target != null)
                    HitTarget(target);
                else
                    ReturnToPool();
            }
        }

        protected override IEffect GetEffect(IDamageable damageable, IEffectable effectable)
        {
            // ��������� PoisonEffect � ����������� ��������� �� DPS
            return new PoisonEffect(poisonEffectDuration, poisonEffectDps, damageable, effectable);
        }

        protected override float GetDamageValue()
        {
            return damage;
        }
    }
}
