using Zenject;
using UnityEngine;
using System;

namespace WannaBe
{
    public class BulletInstaller : MonoInstaller
    {
        [Header("Префаби для GenericBullet")]
        [Tooltip("Префаб кулі типу Fire.")]
        public BaseBullet fireBulletPrefab;
        [Tooltip("Префаб кулі типу Frost.")]
        public BaseBullet frostBulletPrefab;
        [Tooltip("Префаб кулі типу Poison.")]
        public BaseBullet poisonBulletPrefab;

        public int initialPoolSize = 10;

        public override void InstallBindings()
        {
            Container.BindMemoryPool<BaseBullet, BulletPool>()
                .WithId(BulletType.Fire)
                .WithInitialSize(initialPoolSize)
                .FromComponentInNewPrefab(fireBulletPrefab)
                .UnderTransformGroup("Bullets")
                .NonLazy();
            Container.BindMemoryPool<BaseBullet, BulletPool>()
                .WithId(BulletType.Frost)
                .WithInitialSize(initialPoolSize)
                .FromComponentInNewPrefab(frostBulletPrefab)
                .UnderTransformGroup("Bullets")
                .NonLazy();
            Container.BindMemoryPool<BaseBullet, BulletPool>()
                .WithId(BulletType.Poison)
                .WithInitialSize(initialPoolSize)
                .FromComponentInNewPrefab(poisonBulletPrefab)
                .UnderTransformGroup("Bullets")
                .NonLazy();
            Container.BindInterfacesAndSelfTo<BulletDistributor>().AsSingle().NonLazy();
        }
    }
}
