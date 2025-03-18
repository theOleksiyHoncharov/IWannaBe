using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace WannaBe
{
    [RequireComponent(typeof(EnemyAggregator))]
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(EffectListComponent))]
    public class EnemyController : MonoBehaviour, IResettable, IDamageable, IKillable, IGuidable, IEffectable, ITargetMotion
    {
        public EnemyType enemyType;
        public float health = 100f;
        public float speed = 5f;
        public float damage = 10f;

        private bool _isDead = false;

        [SerializeField]
        private NavMeshAgent _navMeshAgent;
        [SerializeField]
        private EffectListComponent _effectComponent;

        [Inject]
        private SignalBus _signalBus;

        // ==== ITargetMotion ====
        public Vector3 Velocity => _navMeshAgent.velocity;

        public void Spawn(Vector3 position)
        {
            transform.position = position;
        }

        public void ResetState()
        {
            health = 100f;
            _isDead = false;
            ResetEffects(); 
        }

        public void Die()
        {
            if (_isDead)
                return;

            _isDead = true;
            _signalBus.Fire(new EnemyDiedSignal { Enemy = this });
        }

        public void Kill()
        {
            Die();
        }

        public void TakeDamage(float damage)
        {
            float multiplier = GetDamageMultiplier();
            float finalDamage = damage * multiplier;
            health -= finalDamage;
            Debug.Log($"{name} отримав {finalDamage} пошкоджень (базово {damage}, множник {multiplier}). Залишилось здоров’я: {health}");

            if (health <= 0)
            {
                Die();
            }
        }

        public void Guide(Vector3 target)
        {
            _navMeshAgent.SetDestination(target);
        }

        // ==== IEffectable ====
        public void ApplyEffect(IEffect effect)
        {
            _effectComponent?.Effects.AddEffect(effect);
        }

        public float GetDamageMultiplier()
        {
            return _effectComponent != null ? _effectComponent.Effects.GetDamageMultiplier() : 1f;
        }

        public float GetMovementMultiplier()
        {
            return _effectComponent != null ? _effectComponent.Effects.GetMovementMultiplier() : 1f;
        }

        public void ResetEffects()
        {
            _effectComponent?.ResetEffects();
        }

        private void UpdateMovementSpeed()
        {
            float baseSpeed = speed;
            float multiplier = GetMovementMultiplier();
            float newSpeed = baseSpeed * multiplier;

            // Використовуємо Mathf.Approximately для порівняння float значень
            if (!Mathf.Approximately(_navMeshAgent.speed, newSpeed))
            {
                _navMeshAgent.speed = newSpeed;
                Debug.Log($"{name} speed updated to: {newSpeed}");
            }
        }

        private void Update()
        {
            UpdateMovementSpeed();
        }
    }
}
