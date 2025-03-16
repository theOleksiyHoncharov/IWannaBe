using System;
using System.Collections.Generic;
using UnityEngine;
namespace WannaBe
{
    [Serializable]
    public class WaveConfig
    {
        [Tooltip("Конфігурації для кожного типу ворога в хвилі")]
        public EnemyWaveConfig[] enemyWaveConfigs;

        [Tooltip("Затримка між спавном окремих ворогів в хвилі")]
        public float delayBetweenSpawns = 0.5f;

        [Tooltip("Затримка між хвилями")]
        public float delayBetweenWaves = 5f;
    }
}