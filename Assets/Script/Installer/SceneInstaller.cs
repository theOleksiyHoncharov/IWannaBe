using System.Collections.Generic;
using UnityEngine;
using Zenject;
namespace WannaBe
{
    public class SceneInstaller : MonoInstaller
    {
        [Header("Player Spawn Settings")]
        [SerializeField] private Transform SlimeSpawnPoint;  // �� �������� ������
        [SerializeField] private GameObject Slime;          // ������ ������ (������)

        public override void InstallBindings()
        {
            // ������� Transform �� ������ �� ���������������, ��� ��������� �� � PlayerSpawner
            Container.Bind<Transform>()
                .WithId("SlimeSpawnPoint")
                .FromInstance(SlimeSpawnPoint)
                .AsSingle();

            Container.Bind<GameObject>()
                .WithId("Slime")
                .FromInstance(Slime)
                .AsSingle();

            // ������� PlayerSpawner, ��� �� ��������� ������ (NonLazy)
            Container.BindInterfacesAndSelfTo<SlimeSpawner>()
                .AsSingle()
                .NonLazy();
            // ��������� FinishPoint � ���� � ������ ���� �� IFinishPointProvider
            Container.Bind<IFinishPointProvider>()
                     .FromComponentInHierarchy()
                     .AsSingle();
            // ���� ����, ���������, �����-������ ��� LevelController
            Container.Bind<SpawnPoint>().FromComponentsInHierarchy().AsCached();
            Container.Bind<LevelController>().FromComponentInHierarchy().AsSingle();
        }
    }
}