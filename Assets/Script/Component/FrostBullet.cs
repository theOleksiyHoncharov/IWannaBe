using UnityEngine;
using Zenject;

namespace WannaBe
{
    public class FrostBullet : BaseBullet
    {
        public override BulletType BulletType => BulletType.Frost;

        [Header("������������ Frost Bullet")]
        [SerializeField] private float initialSpeed = 12f;
        [SerializeField] private float damage = 15f;
        [SerializeField] private float hitThreshold = 0.5f;
        [SerializeField] private float frostEffectDuration = 4f;
        [SerializeField] private float slowMultiplier = 0.7f; // �������� 0.7 ������ �������� �������� �� 30%
        [SerializeField] private float splashRadius = 3f;
        [SerializeField] private LayerMask splashLayerMask;

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
            // ���������� �������� �� �������� ������� �� ������������ �������
            transform.position = startPosition;
        }

        private void Update()
        {
            transform.position = Vector3.MoveTowards(transform.position, _predictedPosition, initialSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _predictedPosition) <= hitThreshold)
            {
                Collider[] hitEnemies = Physics.OverlapSphere(_predictedPosition, splashRadius, splashLayerMask);
                foreach (Collider hit in hitEnemies)
                    HitTarget(hit.transform);
                ReturnToPool();
            }  
        }

        protected override IEffect GetEffect(IDamageable damageable, IEffectable effectable)
        {
            // ��������� FrostEffect, ���� ������ �������� ������
            return new FrostEffect(frostEffectDuration, slowMultiplier);
        }

        protected override float GetDamageValue()
        {
            return damage;
        }
    }
}
