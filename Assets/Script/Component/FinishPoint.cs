using UnityEngine;

namespace WannaBe
{
    public class FinishPoint : MonoBehaviour, IFinishPointProvider
    {
        public Vector3 GetFinishPoint()
        {
            return transform.position;
        }
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<IKillable>()?.Kill();
            }
        }

    }
}