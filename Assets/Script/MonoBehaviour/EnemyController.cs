using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace WannaBe
{
    public class EnemyController : MonoBehaviour, IResettable, IDamageable, IKillable, IGuidable
    {
        public EnemyType enemyType;
        public int health = 100;
        public float speed = 5f;
        public int damage = 10;

        private bool _isDead = false;

        [SerializeField]
        private NavMeshAgent _navMeshAgent;

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
            if (_isDead)
                return;

            _isDead = true; // �� ������ ������� ���� ��������� �� ������ ������ (��������� ����� ������, ��� ���� ��� ���������� �� ���� ���� ��)

            // ³������� ������� ��� ������ ������
            _signalBus.Fire(new EnemyDiedSignal { Enemy = this });
        }

        public void Kill()
        {
            Die();
        }

        public void TakeDamage(int damage)
        {
           this.health -= damage;
            if (this.health <= 0)
            {
                Die();
            }
        }

        public void Guide(Vector3 target)
        {
            _navMeshAgent.SetDestination(target);
        }
    }
}
