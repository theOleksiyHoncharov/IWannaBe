using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
namespace WannaBe
{
    [System.Serializable]
    public class EnemySpawn
    {
        [Tooltip("��� ������������, ���� ���� ����������")]
        public EnemyType enemyType;

        [Tooltip("ʳ������ ������������ ����� ����, �� ����������")]
        public int count;
    }
}