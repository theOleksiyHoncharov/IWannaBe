namespace WannaBe
{
    public interface IMovementModifierEffect
    {
        /// <summary>
        /// �������, ���� ������������� �� ������ ��������.
        /// ���������, 0.8 ������ �������� �������� �� 20%.
        /// </summary>
        float MovementMultiplier { get; }
    }
}