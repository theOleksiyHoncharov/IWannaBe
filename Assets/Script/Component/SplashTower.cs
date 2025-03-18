using System.Collections.Generic;
using UnityEngine;
namespace WannaBe
{
    public class SplashTower : Tower
    {
        protected override Transform GetTarget()
        {
            // Отримуємо всіх ворогів у зоні досяжності
            Collider[] hits = Physics.OverlapSphere(transform.position, targetRange, enemyLayerMask);
            if (hits.Length == 0)
                return null;

            Vector3 finishPoint = finishPointProvider.GetFinishPoint();

            // Групуємо ворогів за відстанню до фінішної точки.
            // Ми використовуємо бакети розміром bucketSize (наприклад, 1f).
            Dictionary<int, List<Transform>> buckets = new Dictionary<int, List<Transform>>();
            float bucketSize = 1f; // Можна налаштувати – визначає, що "приблизно однакова" відстань

            foreach (Collider hit in hits)
            {
                if (!hit.CompareTag("Enemy"))
                    continue;

                Transform enemyTransform = hit.transform;
                // Обчислюємо відстань ворога до фінішної точки
                float distance = Vector3.Distance(enemyTransform.position, finishPoint);
                // Обчислюємо індекс бакета – округлюємо до найближчого цілого
                int bucketIndex = Mathf.RoundToInt(distance / bucketSize);
                if (!buckets.ContainsKey(bucketIndex))
                {
                    buckets[bucketIndex] = new List<Transform>();
                }
                buckets[bucketIndex].Add(enemyTransform);
            }

            if (buckets.Count == 0)
                return null;

            // Знаходимо бакет з найбільшою кількістю ворогів
            int bestBucketIndex = -1;
            int maxCount = 0;
            foreach (var kvp in buckets)
            {
                if (kvp.Value.Count > maxCount)
                {
                    maxCount = kvp.Value.Count;
                    bestBucketIndex = kvp.Key;
                }
            }

            if (bestBucketIndex == -1)
                return null;

            // Вибираємо ворога з найкращого бакета.
            // Наприклад, можемо вибрати того, хто найближче до фінішу.
            Transform bestTarget = null;
            float minDistance = float.MaxValue;
            foreach (Transform enemyTransform in buckets[bestBucketIndex])
            {
                float d = Vector3.Distance(enemyTransform.position, finishPoint);
                if (d < minDistance)
                {
                    minDistance = d;
                    bestTarget = enemyTransform;
                }
            }

            return bestTarget;
        }

    }
}
