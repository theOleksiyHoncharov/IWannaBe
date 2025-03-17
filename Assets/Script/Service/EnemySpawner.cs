using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace WannaBe
{
    // �������� IInitializable ��� ����������� ����������� ����� Zenject,
    // � IDisposable ��� ������� ��� ��������� ������.
    public class EnemySpawner : IInitializable, IDisposable
    {
        [Inject(Id = EnemyType.Goblin)]
        private EnemyControllerPool _goblinPool;

        [Inject(Id = EnemyType.Skeleton)]
        private EnemyControllerPool _skeletonPool;

        [Inject(Id = EnemyType.Zombie)]
        private EnemyControllerPool _zombiePool;

        [Inject]
        private SignalBus _signalBus;  // ���� �� ������������� SignalBus, ������ ���������� ��

        private Dictionary<EnemyType, MemoryPool<EnemyController>> _enemyPools;

        public void Initialize()
        {
            _enemyPools = new Dictionary<EnemyType, MemoryPool<EnemyController>>()
            {
                { EnemyType.Goblin, _goblinPool },
                { EnemyType.Skeleton, _skeletonPool },
                { EnemyType.Zombie, _zombiePool }
            };

            // ϳ������ �� ������ ����� ������
            _signalBus.Subscribe<EnemyDiedSignal>(OnEnemyDied);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<EnemyDiedSignal>(OnEnemyDied);
        }

        private void OnEnemyDied(EnemyDiedSignal signal)
        {
            ReturnEnemy(signal.Enemy);
        }

        /// <summary>
        /// �������� ������ �������� ���� �� ������� �������.
        /// </summary>
        public EnemyController SpawnEnemy(EnemyType type, Vector3 position, Vector3 guidePoint)
        {
            if (_enemyPools.TryGetValue(type, out var pool))
            {
                EnemyController enemy = pool.Spawn();
                enemy.enemyType = type;
                enemy.Spawn(position);
                enemy.Guide(guidePoint);
                return enemy;
            }
            else
            {
                Debug.LogError($"��� ��� ���� {type} �� ��������!");
                return null;
            }
        }

        /// <summary>
        /// ������� ������ �� ����.
        /// </summary>
        public void ReturnEnemy(EnemyController enemy)
        {
            if (_enemyPools.TryGetValue(enemy.enemyType, out var pool))
            {
                pool.Despawn(enemy);
            }
            else
            {
                Debug.LogError($"��� ��� ���� {enemy.enemyType} �� ��������!");
            }
        }
    }
}
