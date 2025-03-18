using UnityEngine;
using Zenject;
using System.Collections.Generic;

namespace WannaBe
{
    public class Tower : MonoBehaviour
    {
        [Header("Налаштування стрільби")]
        [Tooltip("Точка, з якої здійснюється стрільба.")]
        [SerializeField] private Transform firePoint;
        [Tooltip("Кількість пострілів за секунду.")]
        [SerializeField] private float fireRate = 1f;
        private float fireCooldown;

        [Header("Налаштування таргетингу")]
        [Tooltip("Радіус досяжності для виявлення ворогів.")]
        [SerializeField] private float targetRange = 10f;
        [Tooltip("Шар, на якому знаходяться вороги.")]
        [SerializeField] private LayerMask enemyLayerMask;

        [Header("Налаштування кулі")]
        [Tooltip("Тип кулі, що буде використовуватись при стрільбі.")]
        [SerializeField] private BulletType bulletType;

        // Інжектимо дистриб'ютора куль і провайдера фінішної точки
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
        /// Виявляє ворогів у радіусі досяжності та повертає того, хто найближче до фінішної точки.
        /// </summary>
        /// <returns>Transform цільового ворога або null, якщо ворогів немає.</returns>
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
        /// Стріляє у ціль, отримуючи кулю з дистриб'ютора.
        /// </summary>
        /// <param name="target">Transform обраного ворога.</param>
        private void Fire(Transform target)
        {
            if (firePoint == null)
            {
                Debug.LogWarning("FirePoint не встановлено!");
                return;
            }
            IBullet bullet = bulletDistributor.GetBullet(bulletType);
            if (bullet != null)
            {
                // Якщо IBullet реалізовано як MonoBehaviour, встановлюємо позицію та орієнтацію
                if (bullet is MonoBehaviour bulletMono)
                {
                    bulletMono.transform.position = firePoint.position;
                    bulletMono.transform.rotation = firePoint.rotation;
                }
                // Ініціалізація кулі із стартовою позицією та Transform ворога
                bullet.Initialize(firePoint.position, target);
            }
        }
    }
}
