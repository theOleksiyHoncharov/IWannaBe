using Zenject.SpaceFighter;

namespace WannaBe
{
    /// <summary>
    /// ��������� ��� ������'����� ����.
    /// </summary>
    public interface IBulletDistributor
    {
        /// <summary>
        /// ������� ��������� ��� �������� ����.
        /// </summary>
        IBullet GetBullet(BulletType bulletType);
    }
}