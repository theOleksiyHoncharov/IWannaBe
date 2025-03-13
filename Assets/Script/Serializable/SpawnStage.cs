using System.Collections.Generic;
using UnityEngine;
namespace WannaBe
{
    [System.Serializable]
    public class SpawnStage
    {
        [Tooltip("���, ���� ���������� ����� ����, � ��������")]
        public float spawnTime;

        [Tooltip("������ ���� ������������, �� �'�������� � ��� ����")]
        public List<EnemySpawn> enemySpawns = new List<EnemySpawn>();
    }
}