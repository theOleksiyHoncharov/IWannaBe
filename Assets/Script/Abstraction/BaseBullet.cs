using UnityEngine;
using Zenject;

namespace WannaBe
{
    public abstract class BaseBullet : MonoBehaviour, IBullet
    {
        [SerializeField]
        private BulletType bulletType;
        public BulletType BulletType => bulletType;

        protected SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public abstract void Initialize(Vector3 startPosition, Transform enemyTransform);

        /// <summary>
        /// Метод для повернення кулі в пул через Zenject-сигнал.
        /// Викликайте його, коли куля має бути повернена (наприклад, після зіткнення або закінчення життя).
        /// </summary>
        protected void ReturnToPool()
        {
            _signalBus.Fire(new BulletReturnSignal { Bullet = this });
        }
    }
}
