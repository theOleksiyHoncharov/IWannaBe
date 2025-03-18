using UnityEngine;
using Zenject;
using System.Collections.Generic;

namespace WannaBe
{
    public class Tower : MonoBehaviour
    {
        [Header("������������ �������")]
        [Tooltip("�����, � ��� ����������� �������.")]
        [SerializeField] private Transform firePoint;
        [Tooltip("ʳ������ ������� �� �������.")]
        [SerializeField] private float fireRate = 1f;
        private float fireCooldown;

        [Header("������������ ����������")]
        [Tooltip("����� ��������� ��� ��������� ������.")]
        [SerializeField] private float targetRange = 10f;
        [Tooltip("���, �� ����� ����������� ������.")]
        [SerializeField] private LayerMask enemyLayerMask;

        [Header("������������ ���")]
        [Tooltip("��� ���, �� ���� ����������������� ��� ������.")]
        [SerializeField] private BulletType bulletType;

        // ��������� �������'����� ���� � ���������� ������ �����
        [Inject] private IBulletDistributor bulletDistributor;
        [Inject] private IFinishPointProvider finishPointProvider;

        void Update()
        {
            if (fireCooldown <= 0f)
            {
                Transform target = GetTarget();
                if (target != null)
                {
                    Fire(target);
                    fireCooldown = 1f / fireRate;
                }
            }
            else
            {
                fireCooldown -= Time.deltaTime;
            }
        }

        /// <summary>
        /// ������� ������ � ����� ��������� �� ������� ����, ��� ��������� �� ������ �����.
        /// </summary>
        /// <returns>Transform ��������� ������ ��� null, ���� ������ ����.</returns>
        private Transform GetTarget()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, targetRange, enemyLayerMask);
            if (hits.Length == 0)
                return null;

            Vector3 finishPoint = finishPointProvider.GetFinishPoint();
            Transform bestTarget = null;
            float minDistance = float.MaxValue;

            foreach (Collider hit in hits)
            {
                if (!hit.CompareTag("Enemy"))
                    continue;

                Transform enemyTransform = hit.transform;
                float distanceToFinish = Vector3.Distance(enemyTransform.position, finishPoint);
                if (distanceToFinish < minDistance)
                {
                    minDistance = distanceToFinish;
                    bestTarget = enemyTransform;
                }
            }
            return bestTarget;
        }

        /// <summary>
        /// ������ � ����, ��������� ���� � �������'�����.
        /// </summary>
        /// <param name="target">Transform �������� ������.</param>
        private void Fire(Transform target)
        {
            if (firePoint == null)
            {
                Debug.LogWarning("FirePoint �� �����������!");
                return;
            }
            IBullet bullet = bulletDistributor.GetBullet(bulletType);
            if (bullet != null)
            {
                // ���� IBullet ���������� �� MonoBehaviour, ������������ ������� �� ��������
                if (bullet is MonoBehaviour bulletMono)
                {
                    bulletMono.transform.position = firePoint.position;
                    bulletMono.transform.rotation = firePoint.rotation;
                }
                // ����������� ��� �� ��������� �������� �� Transform ������
                bullet.Initialize(firePoint.position, target);
            }
        }
    }
}
