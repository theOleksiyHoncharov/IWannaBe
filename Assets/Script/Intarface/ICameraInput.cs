using UnityEngine;
namespace WannaBe
{
    public interface ICameraInput
    {
        /// <summary>
        /// ������� �������� ������� ���� (Vector2).
        /// </summary>
        Vector2 GetRightStickValue();
    }
}