using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WannaBe
{
    public class LevelController : MonoBehaviour
    {
        [Inject]
        private EnemySpawner _enemySpawner;

        // Список спавн-пойнтів на сцені
        [Inject]
        private List<SpawnPoint> _spawnPoints;

        [Tooltip("Конфігурації хвиль, що задаються через інспектор")]
        public WaveConfig[] waveConfigs;

        private void Start()
        {
            StartCoroutine(SpawnWaves());
        }

        private IEnumerator SpawnWaves()
        {
            foreach (var wave in waveConfigs)
            {
                // Для кожного типу ворога в хвилі спавнимо задану кількість
                foreach (var config in wave.enemyWaveConfigs)
                {
                    for (int i = 0; i < config.count; i++)
                    {
                        SpawnPoint spawnPoint = GetRandomSpawnPoint();
                        if (spawnPoint != null)
                        {
                            _enemySpawner.SpawnEnemy(config.enemyType, spawnPoint.transform.position);
                        }
                        yield return new WaitForSeconds(wave.delayBetweenSpawns);
                    }
                }
                yield return new WaitForSeconds(wave.delayBetweenWaves);
            }
        }

        private SpawnPoint GetRandomSpawnPoint()
        {
            if (_spawnPoints != null && _spawnPoints.Count > 0)
            {
                int index = Random.Range(0, _spawnPoints.Count);
                return _spawnPoints[index];
            }
            return null;
        }
    }
}
