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
            // ��������� �����������
        }

        public void ResetState()
        {
            health = 100;
            // �������� ���������� ���������
        }

        public void Die()
        {
            // ����� ����� ������, ���������, ���������� �������, ������ ����

            // ³������� ������� ��� ������ ������
            _signalBus.Fire(new EnemyDiedSignal { Enemy = this });
        }
        public class Pool : MemoryPool<EnemyController>
        {
            // �����������, ���� ��'��� ���������� � ����
            protected override void OnSpawned(EnemyController enemy)
            {
                base.OnSpawned(enemy);
                enemy.ResetState();
                enemy.gameObject.SetActive(true);
            }

            // �����������, ���� ��'��� ����������� � ���
            protected override void OnDespawned(EnemyController enemy)
            {
                base.OnDespawned(enemy);
                enemy.gameObject.SetActive(false);
            }
        }
    }
}
