using System;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
namespace WannaBe
{
    [Serializable]
    public class EnemyWaveConfig
    {
        public EnemyType enemyType;
        public int count;
    }
}