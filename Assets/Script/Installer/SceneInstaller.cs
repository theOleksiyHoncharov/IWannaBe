using UnityEngine;
using Zenject;

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
    }
}
