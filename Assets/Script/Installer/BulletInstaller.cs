using Zenject;
using UnityEngine;
using System;

namespace WannaBe
{
    public class BulletInstaller : MonoInstaller
    {
        [Header("������� ��� GenericBullet")]
        [Tooltip("������ ��� ���� Fire.")]
        public BaseBullet fireBulletPrefab;

        public int initialPoolSize = 10;

        public override void InstallBindings()
        {
            Container.BindMemoryPool<BaseBullet, BulletPool>()
                .WithId(BulletType.Fire)
                .WithInitialSize(initialPoolSize)
                .FromComponentInNewPrefab(fireBulletPrefab)
                .UnderTransformGroup("Bullets")
                .NonLazy();

            Container.BindInterfacesAndSelfTo<BulletDistributor>().AsSingle().NonLazy();
        }
    }
}
