using UnityEngine;
using Zenject;

public class SlimeSpawner : IInitializable
{
    private readonly Transform _slimeSpawnPoint;
    private readonly GameObject _slimePrefab;
    private readonly DiContainer _container;

    // Отримуємо залежності через інжект (ідентифікатори збігаються з SceneInstaller)
    public SlimeSpawner(
        [Inject(Id = "SlimeSpawnPoint")] Transform slimeSpawnPoint,
        [Inject(Id = "Slime")] GameObject slimePrefab,
        DiContainer container)
    {
        _slimeSpawnPoint = slimeSpawnPoint;
        _slimePrefab = slimePrefab;
        _container = container;
    }

    // Цей метод викличеться одразу після встановлення всіх біндінгів
    public void Initialize()
    {
        // Спавнимо префаб, використовуючи Zenject (збережемо посилання, якщо треба)
        _container.InstantiatePrefab(
            _slimePrefab,
            _slimeSpawnPoint.position,
            _slimeSpawnPoint.rotation,
            null
        );

        Debug.Log("[PlayerSpawner] Slime spawned!");
    }
}
