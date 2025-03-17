using System;
using System.Collections.Generic;
using Zenject;
using UnityEngine;

namespace WannaBe
{
    /// <summary>
    /// Дистриб'ютор, який використовує пул, зареєстрований по ID (енуму),
    /// для отримання та повернення кулі через сигнал.
    /// </summary>
    public class BulletDistributor : IBulletDistributor, IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly Dictionary<BulletType, BulletPool> _pools;

        public BulletDistributor(
            [Inject(Id = BulletType.Fire)] BulletPool fireBulletPool,
            SignalBus signalBus)
        {
            _signalBus = signalBus;
            _pools = new Dictionary<BulletType, BulletPool>
            {
                { BulletType.Fire, fireBulletPool }
            };
        }

        public void Initialize()
        {
            _signalBus.Subscribe<BulletReturnSignal>(OnBulletReturn);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<BulletReturnSignal>(OnBulletReturn);
        }

        public IBullet GetBullet(BulletType bulletType)
        {
            if (_pools.TryGetValue(bulletType, out var pool))
            {
                return pool.Spawn();
            }

            Debug.LogError($"Пул для типу кулі {bulletType} не знайдено!");
            return null;
        }

        private void OnBulletReturn(BulletReturnSignal signal)
        {
            if (signal.Bullet is BaseBullet bullet)
            {
                if (_pools.TryGetValue(bullet.BulletType, out var pool))
                {
                    pool.Despawn(bullet);
                }
                else
                {
                    Debug.LogError($"Пул для типу {bullet.BulletType} не знайдено при поверненні кулі.");
                }
            }
        }
    }
}
