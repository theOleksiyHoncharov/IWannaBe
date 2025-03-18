namespace WannaBe
{
    public interface IEffectable
    {
        /// <summary>
        /// ������� ����� ����� ��� ������� ��� ��������.
        /// </summary>
        void ApplyEffect(IEffect effect);

        /// <summary>
        /// ������� ������� ���������� � ������.
        /// </summary>
        float GetDamageMultiplier();

        /// <summary>
        /// ������� ������� �������� � ������.
        /// </summary>
        float GetMovementMultiplier();

        /// <summary>
        /// ����� �� ������ ������ (���������, ��� ��������� � ���).
        /// </summary>
        void ResetEffects();
    }
}
