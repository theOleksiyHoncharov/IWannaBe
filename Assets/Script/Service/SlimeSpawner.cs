using UnityEngine;
using Zenject;

public class SlimeSpawner : IInitializable
{
    private readonly Transform _slimeSpawnPoint;
    private readonly GameObject _slimePrefab;
    private readonly DiContainer _container;

    // �������� ��������� ����� ������ (�������������� ��������� � SceneInstaller)
    public SlimeSpawner(
        [Inject(Id = "SlimeSpawnPoint")] Transform slimeSpawnPoint,
        [Inject(Id = "Slime")] GameObject slimePrefab,
        DiContainer container)
    {
        _slimeSpawnPoint = slimeSpawnPoint;
        _slimePrefab = slimePrefab;
        _container = container;
    }

    // ��� ����� ����������� ������ ���� ������������ ��� ������
    public void Initialize()
    {
        // �������� ������, �������������� Zenject (��������� ���������, ���� �����)
        _container.InstantiatePrefab(
            _slimePrefab,
            _slimeSpawnPoint.position,
            _slimeSpawnPoint.rotation,
            null
        );

        Debug.Log("[PlayerSpawner] Slime spawned!");
    }
}
