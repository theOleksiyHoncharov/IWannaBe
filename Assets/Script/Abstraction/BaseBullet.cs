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
        /// ����� ��� ���������� ��� � ��� ����� Zenject-������.
        /// ���������� ����, ���� ���� �� ���� ��������� (���������, ���� �������� ��� ��������� �����).
        /// </summary>
        protected void ReturnToPool()
        {
            _signalBus.Fire(new BulletReturnSignal { Bullet = this });
        }
    }
}
