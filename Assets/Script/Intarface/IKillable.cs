namespace WannaBe
{
    public interface IKillable
    {
        /// <summary>
        /// Викликається, коли ворог досягає фінішної точки (KillBox)
        /// </summary>
        void Kill();
    }
}