using Zenject;
using UnityEngine;

namespace WannaBe
{
    public class EnemyInstaller : MonoInstaller
    {
        public EnemyController goblinPrefab;
        public EnemyController skeletonPrefab;
        public EnemyController zombiePrefab;
        public int initialPoolSize = 10;

        public override void InstallBindings()
        {
            // ����'����� ��� ��� ������
            Container.BindMemoryPool<EnemyController, EnemyControllerPool>()
                .WithId(EnemyType.Goblin)
                .WithInitialSize(initialPoolSize)
                .FromComponentInNewPrefab(goblinPrefab)
                .UnderTransformGroup("EnemyPool")
                .NonLazy();

            // ����'����� ��� ��� ���������
            Container.BindMemoryPool<EnemyController, EnemyControllerPool>()
                .WithId(EnemyType.Skeleton)
                .WithInitialSize(initialPoolSize)
                .FromComponentInNewPrefab(skeletonPrefab)
                .UnderTransformGroup("EnemyPool")
                .NonLazy();

            // ����'����� ��� ��� ����
            Container.BindMemoryPool<EnemyController, EnemyControllerPool>()
                .WithId(EnemyType.Zombie)
                .WithInitialSize(initialPoolSize)
                .FromComponentInNewPrefab(zombiePrefab)
                .UnderTransformGroup("EnemyPool")
                .NonLazy();

            // ������������ EnemySpawner 
            Container.BindInterfacesAndSelfTo<EnemySpawner>()
                .AsSingle()
                .NonLazy();
        }
    }
}
