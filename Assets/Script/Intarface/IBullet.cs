using UnityEngine;

namespace WannaBe
{
    public interface IBullet
    {
        /// <summary>
        /// �������� ����, ������� �������� ����, ���� ������������ �� ������ �� �������� ��� �� ��������� ��������.
        /// ����� ���������� �������� ��� �������� �� ��������.
        /// </summary>
        /// <param name="startPosition">��������� ������� ���.</param>
        /// <param name="enemyTransform">Transform ������, �� ����� ������ ����.</param>
        void Initialize(Vector3 startPosition, Transform enemyTransform);
    }
}