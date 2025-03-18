using UnityEngine;

namespace WannaBe
{
    // ���������-��������� ��� ������, �� ���� ������ ����������.
    public class EnemyAggregator : MonoBehaviour
    {
        public IDamageable Damageable { get; private set; }
        public IEffectable Effectable { get; private set; }

        private void Awake()
        {
            // ������ ��������� �� ����������. ����������, �� ���� ����������� �� ���� � GameObject.
            Damageable = GetComponent<IDamageable>();
            Effectable = GetComponent<IEffectable>();
        }
    }
}
