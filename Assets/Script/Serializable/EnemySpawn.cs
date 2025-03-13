using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
namespace WannaBe
{
    [System.Serializable]
    public class EnemySpawn
    {
        [Tooltip("Тип супротивника, який буде спавнитись")]
        public EnemyType enemyType;

        [Tooltip("Кількість супротивників цього типу, що спавняться")]
        public int count;
    }
}