using UnityEngine;
using Zenject;

namespace WannaBe
{
    public abstract class BaseBullet : MonoBehaviour, IBullet
    {
        public abstract BulletType BulletType { get; }

        protected SignalBus _signalBus;
        protected Transform target;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        /// <summary>
        /// Ініціалізує кулю: встановлює початкову позицію і орієнтацію, 
        /// визначає траєкторію – специфічно для конкретного типу кулі.
        /// </summary>
        public abstract void Initialize(Vector3 startPosition, Transform enemyTransform);

        /// <summary>
        /// Повертає ефект, який ця куля накладає на ворога.
        /// </summary>
        protected abstract IEffect GetEffect();

        /// <summary>
        /// Повертає значення пошкодження, яке ця куля завдає.
        /// </summary>
        protected abstract float GetDamageValue();

        /// <summary>
        /// Загальна логіка обробки попадання: 
        /// застосування ефекту, нанесення пошкодження через агрегатор ворога та повернення кулі в пул.
        /// </summary>
        protected void HitTarget(Transform target)
        {
            if (target != null)
            {
                var aggregator = target.GetComponent<EnemyAggregator>();
                if (aggregator != null)
                {
                    aggregator.Effectable?.ApplyEffect(GetEffect());
                    aggregator.Damageable?.TakeDamage(GetDamageValue());
                }
            }

            ResetBullet(); // Гарантуємо, що перед поверненням куля скине стан
            ReturnToPool();
        }

        /// <summary>
        /// Скидає стан кулі перед поверненням в пул.
        /// </summary>
        public virtual void ResetBullet()
        {
            target = null;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Повертає кулю в пул шляхом надсилання Zenject-сигналу.
        /// </summary>
        protected void ReturnToPool()
        {
            _signalBus.Fire(new BulletReturnSignal { Bullet = this });
        }
    }
}
