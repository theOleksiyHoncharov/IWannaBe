using System.Collections.Generic;
using UnityEngine;
namespace WannaBe
{
    [System.Serializable]
    public class SpawnStage
    {
        [Tooltip("Час, коли відбувається спавн хвилі, в секундах")]
        public float spawnTime;

        [Tooltip("Список типів супротивників, які з'являться в цій хвилі")]
        public List<EnemySpawn> enemySpawns = new List<EnemySpawn>();
    }
}