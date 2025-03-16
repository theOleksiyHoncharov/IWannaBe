using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace WannaBe
{
    public class SceneInstaller : MonoInstaller
    {
        [Header("Player Spawn Settings")]
        [SerializeField] private Transform SlimeSpawnPoint;  // Де спавнити гравця
        [SerializeField] private GameObject Slime;          // Префаб гравця (слайма)

        public override void InstallBindings()
        {
            // Зв’язуємо Transform та префаб по ідентифікаторах, щоб інжектити їх у PlayerSpawner
            Container.Bind<Transform>()
                .WithId("SlimeSpawnPoint")
                .FromInstance(SlimeSpawnPoint)
                .AsSingle();

            Container.Bind<GameObject>()
                .WithId("Slime")
                .FromInstance(Slime)
                .AsSingle();

            // Зв’язуємо PlayerSpawner, щоб він виконався одразу (NonLazy)
            Container.BindInterfacesAndSelfTo<SlimeSpawner>()
                .AsSingle()
                .NonLazy();
            // Знаходимо FinishPoint у сцені і біндуємо його як IFinishPointProvider
            Container.Bind<IFinishPointProvider>()
                     .FromComponentInHierarchy()
                     .AsSingle();
            // Інші бінди, наприклад, спавн-пойнти або LevelController
            Container.Bind<SpawnPoint>().FromComponentsInHierarchy().AsCached();
            Container.Bind<LevelController>().FromComponentInHierarchy().AsSingle();
        }
    }
}