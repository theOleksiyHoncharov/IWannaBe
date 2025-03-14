using UnityEngine;
using UnityEngine.AI;

namespace WannaBe
{
    public class EnemyController : MonoBehaviour
    {
        private NavMeshAgent agent;

        [Tooltip("Фінальна точка, до якої має рухатися ворог")]
        public Transform finalDestination;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            // Запуск behavior tree, наприклад, встановлюємо ціль
            StartMovement();
        }

        private void StartMovement()
        {
            if (finalDestination != null)
            {
                // Завдання SetDestination: встановлення цілі
                agent.SetDestination(finalDestination.position);

                // Тут можна інтегрувати виклик інших вузлів behavior tree, наприклад, WaitForArrival
            }
            else
            {
                Debug.LogWarning("Фінальна точка не встановлена для " + gameObject.name);
            }
        }
    }
}