namespace WannaBe
{
    public interface IEffect
    {
        /// <summary>
        /// ��������� ������ (����� ��� ������� �����)
        /// </summary>
        float Duration { get; }

        /// <summary>
        /// ��� ������ (��� ���������, �� ����� ��� ���������)
        /// </summary>
        EffectType Type { get; }

        /// <summary>
        /// �����, ���� ������ ��� 䳿 ������. 
        /// ����������� ������� ���.
        /// </summary>
        void UpdateEffect(float deltaTime);

        /// <summary>
        /// �������, �� ���������� �����
        /// </summary>
        bool IsExpired { get; }

        /// <summary>
        /// ��'���� ����� ����� �� ��� ���������� (���������, ������� Duration ��� ������ ���������)
        /// </summary>
        /// <param name="other">����� ����� ���� ������ ����</param>
        void Merge(IEffect other);

        /// <summary>
        /// �����������, ���� ����� �����������
        /// </summary>
        void OnApply();

        /// <summary>
        /// �����������, ���� ����� ��������
        /// </summary>
        void OnRemove();
    }
}
