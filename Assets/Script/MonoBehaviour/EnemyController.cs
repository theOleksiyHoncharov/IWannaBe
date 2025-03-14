using UnityEngine;
using UnityEngine.AI;

namespace WannaBe
{
    public class EnemyController : MonoBehaviour
    {
        private NavMeshAgent agent;

        [Tooltip("Գ������ �����, �� ��� �� �������� �����")]
        public Transform finalDestination;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Start()
        {
            // ������ behavior tree, ���������, ������������ ����
            StartMovement();
        }

        private void StartMovement()
        {
            if (finalDestination != null)
            {
                // �������� SetDestination: ������������ ���
                agent.SetDestination(finalDestination.position);

                // ��� ����� ����������� ������ ����� ����� behavior tree, ���������, WaitForArrival
            }
            else
            {
                Debug.LogWarning("Գ������ ����� �� ����������� ��� " + gameObject.name);
            }
        }
    }
}