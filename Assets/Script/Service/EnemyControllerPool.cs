using Zenject;
using UnityEngine;

namespace WannaBe
{
    public class EnemyControllerPool : MemoryPool<EnemyController>
    {
        protected override void Reinitialize(EnemyController enemy)
        {
            enemy.ResetState();
            enemy.gameObject.SetActive(true);
        }

        protected override void OnDespawned(EnemyController enemy)
        {
            enemy.gameObject.SetActive(false);
        }
    }
}
