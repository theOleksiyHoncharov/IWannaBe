using UnityEngine;

namespace WannaBe
{
    public class FinishPoint : MonoBehaviour, IFinishPointProvider
    {
        public UnityEngine.Transform GetFinishPoint()
        {
            return transform;
        }
    }
}