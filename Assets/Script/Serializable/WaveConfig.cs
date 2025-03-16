using System;
using System.Collections.Generic;
using UnityEngine;
namespace WannaBe
{
    [Serializable]
    public class WaveConfig
    {
        [Tooltip("������������ ��� ������� ���� ������ � ����")]
        public EnemyWaveConfig[] enemyWaveConfigs;

        [Tooltip("�������� �� ������� ������� ������ � ����")]
        public float delayBetweenSpawns = 0.5f;

        [Tooltip("�������� �� �������")]
        public float delayBetweenWaves = 5f;
    }
}