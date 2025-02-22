using UnityEngine;
using Zenject;

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
    }
}
