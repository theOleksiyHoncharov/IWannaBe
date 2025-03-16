using UnityEngine;
using Zenject;

namespace WannaBe
{
    public class EnemyController : MonoBehaviour, IResettable
    {
        public EnemyType enemyType;
        public int health = 100;
        public float speed = 5f;
        public int damage = 10;


        [Inject]
        private SignalBus _signalBus;

        public void Spawn(Vector3 position)
        {
            transform.position = position;
            // Додаткова ініціалізація
        }

        public void ResetState()
        {
            health = 100;
            // Скидання додаткових параметрів
        }

        public void Die()
        {
            // Логіка смерті ворога, наприклад, відтворення анімації, ефектів тощо

            // Відправка сигналу про смерть ворога
            _signalBus.Fire(new EnemyDiedSignal { Enemy = this });
        }
        public class Pool : MemoryPool<EnemyController>
        {
            // Викликається, коли об'єкт отримується з пулу
            protected override void OnSpawned(EnemyController enemy)
            {
                base.OnSpawned(enemy);
                enemy.ResetState();
                enemy.gameObject.SetActive(true);
            }

            // Викликається, коли об'єкт повертається в пул
            protected override void OnDespawned(EnemyController enemy)
            {
                base.OnDespawned(enemy);
                enemy.gameObject.SetActive(false);
            }
        }
    }
}
