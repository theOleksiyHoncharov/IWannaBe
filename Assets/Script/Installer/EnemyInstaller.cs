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
            // Прив'язуємо пул для Гобліна
            Container.BindMemoryPool<EnemyController, EnemyControllerPool>()
                .WithId(EnemyType.Goblin)
                .WithInitialSize(initialPoolSize)
                .FromComponentInNewPrefab(goblinPrefab)
                .UnderTransformGroup("EnemyPool")
                .NonLazy();

            // Прив'язуємо пул для Скелетона
            Container.BindMemoryPool<EnemyController, EnemyControllerPool>()
                .WithId(EnemyType.Skeleton)
                .WithInitialSize(initialPoolSize)
                .FromComponentInNewPrefab(skeletonPrefab)
                .UnderTransformGroup("EnemyPool")
                .NonLazy();

            // Прив'язуємо пул для Зомбі
            Container.BindMemoryPool<EnemyController, EnemyControllerPool>()
                .WithId(EnemyType.Zombie)
                .WithInitialSize(initialPoolSize)
                .FromComponentInNewPrefab(zombiePrefab)
                .UnderTransformGroup("EnemyPool")
                .NonLazy();

            // Зареєструвати EnemySpawner 
            Container.BindInterfacesAndSelfTo<EnemySpawner>()
                .AsSingle()
                .NonLazy();
        }
    }
}
