using UnityEngine;
using Zenject;

namespace WannaBe
{
    public class BulletPool : MemoryPool<BaseBullet>
    {
        protected override void OnCreated(BaseBullet bullet)
        {
            bullet.gameObject.SetActive(false);
            base.OnCreated(bullet);
        }

        protected override void OnSpawned(BaseBullet bullet)
        {
            bullet.gameObject.SetActive(true);
            base.OnSpawned(bullet);
        }

        protected override void OnDespawned(BaseBullet bullet)
        {
            bullet.gameObject.SetActive(false);
            base.OnDespawned(bullet);
        }
    }
}
