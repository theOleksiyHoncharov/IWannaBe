using UnityEngine;

namespace WannaBe
{
    // Компонент-агрегатор для ворога, що кешує потрібні інтерфейси.
    public class EnemyAggregator : MonoBehaviour
    {
        public IDamageable Damageable { get; private set; }
        public IEffectable Effectable { get; private set; }

        private void Awake()
        {
            // Кешуємо посилання на інтерфейси. Припускаємо, що вони знаходяться на тому ж GameObject.
            Damageable = GetComponent<IDamageable>();
            Effectable = GetComponent<IEffectable>();
        }
    }
}
