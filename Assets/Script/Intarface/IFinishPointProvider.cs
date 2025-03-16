using UnityEngine;

namespace WannaBe
{
    public interface IFinishPointProvider
    {
        /// <summary>
        /// ������� �������, ��� � ����� ��� ������.
        /// </summary>
        Vector3 GetFinishPoint();
    }
}