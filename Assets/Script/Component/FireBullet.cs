using UnityEngine;
using Zenject;

namespace WannaBe
{
    public class FireBullet : BaseBullet
    {
        public override BulletType BulletType => BulletType.Fire;

        [Header("Налаштування Fire Bullet")]
        [SerializeField] private float initialSpeed = 15f;
        [SerializeField] private float damage = 20f;
        [SerializeField] private float hitThreshold = 0.5f; // Відстань, при якій вважаємо, що куля влучила
        [SerializeField] private float fireEffectDuration = 5f;
        [SerializeField] private float fireEffectMultiplier = 1.5f;

        private Transform target;

        private Vector3 _predictedPosition;
        /// <summary>
        /// Ініціалізує кулю.
        /// Обчислює прогнозовану позицію цілі з урахуванням її швидкості (якщо вона доступна через ITargetMotion),
        /// встановлює початкову позицію і орієнтацію кулі.
        /// </summary>
        public override void Initialize(Vector3 startPosition, Transform enemyTransform)
        {
            transform.rotation = Quaternion.identity;
            target = enemyTransform;

            // Отримуємо швидкість цілі через ITargetMotion
            Vector3 targetVelocity = Vector3.zero;
            var targetMotion = enemyTransform.GetComponent<ITargetMotion>();
            if (targetMotion != null)
            {
                targetVelocity = targetMotion.Velocity;
            }

            // Обчислюємо відстань від стартової точки до поточної позиції ворога
            float distance = Vector3.Distance(startPosition, enemyTransform.position);
            // Приблизний час польоту кулі
            float flightTime = distance / initialSpeed;
            // Прогнозована позиція ворога
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

        /// <summary>
        /// Повертає ефект, який накладає FireBullet (наприклад, FireEffect).
        /// </summary>
        protected override IEffect GetEffect()
        {
            return new FireEffect(fireEffectDuration, fireEffectMultiplier);
        }

        /// <summary>
        /// Повертає значення пошкодження, яке завдає FireBullet.
        /// </summary>
        protected override float GetDamageValue()
        {
            return damage;
        }
    }
}
