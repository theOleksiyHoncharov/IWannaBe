using Zenject.SpaceFighter;

namespace WannaBe
{
    /// <summary>
    /// Інтерфейс для дістриб'ютора куль.
    /// </summary>
    public interface IBulletDistributor
    {
        /// <summary>
        /// Повертає екземпляр кулі заданого типу.
        /// </summary>
        IBullet GetBullet(BulletType bulletType);
    }
}