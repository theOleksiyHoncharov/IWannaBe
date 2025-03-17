using UnityEngine;
using Zenject;

namespace WannaBe
{
    public class FireBullet : BaseBullet, IBullet
    {
        [Header("Налаштування Fire Bullet")]
        [SerializeField] private float initialSpeed = 15f;
        [SerializeField] private float homingSpeed = 10f;  // (можна використати для більшої плавності, якщо потрібно)
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float damage = 20f;
        [SerializeField] private float hitThreshold = 0.5f; // Відстань, при якій вважаємо, що куля влучила

        // Зберігаємо ціль для подальшого використання в Update
        private Transform target;

        /// <summary>
        /// Ініціалізує кулю: задає стартову позицію, обчислює напрямок до ворога, встановлює початкову швидкість та орієнтацію.
        /// </summary>
        /// <param name="startPosition">Стартова позиція кулі.</param>
        /// <param name="enemyTransform">Transform ворога, до якого летить куля.</param>
        public override void Initialize(Vector3 startPosition, Transform enemyTransform)
        {
            target = enemyTransform;

            // Обчислюємо напрямок до ворога від стартової позиції
            Vector3 direction = (enemyTransform.position - startPosition).normalized;

            // Встановлюємо початкову позицію та орієнтацію кулі
            transform.position = startPosition;
            transform.rotation = Quaternion.LookRotation(direction);

            // Встановлюємо початкову швидкість кулі (якщо використовується Rigidbody)
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = direction * initialSpeed;
            }
        }

        private void Update()
        {
            if (target != null)
            {
                // Гарантовано переміщуємо кулю до поточної позиції ворога
                transform.position = Vector3.MoveTowards(transform.position, target.position, initialSpeed * Time.deltaTime);

                // Оновлюємо орієнтацію кулі, щоб вона завжди дивилася на ворога
                Vector3 newDirection = (target.position - transform.position).normalized;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(newDirection), Time.deltaTime * rotationSpeed);

                // Якщо куля досягла ворога (або досить близько), наносимо пошкодження
                if (Vector3.Distance(transform.position, target.position) <= hitThreshold)
                {
                    HitTarget();
                }
            }
            else
            {
                // Якщо ціль недоступна, рухаємо кулю у поточному напрямку
                transform.position += transform.forward * initialSpeed * Time.deltaTime;
            }
        }

        /// <summary>
        /// Обробка попадання в ціль: наносить пошкодження ворогу і повертає кулю в пул.
        /// </summary>
        private void HitTarget()
        {
            if (target != null)
            {
                var damageable = target.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage((int)damage);
                }
            }
            ReturnToPool();
        }

        private void OnTriggerEnter(Collider other)
        {
            // Додатковий захід: якщо куля зіткнеться з об'єктом з тегом "Enemy"
            if (other.CompareTag("Enemy"))
            {
                var damageable = other.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage((int)damage);
                }
                ReturnToPool();
            }
        }
    }
}
