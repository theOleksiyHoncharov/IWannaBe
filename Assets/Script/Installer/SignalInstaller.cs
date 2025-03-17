using Zenject;

namespace WannaBe
{
    public class SignalInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<EnemyDiedSignal>();
            Container.DeclareSignal<BulletReturnSignal>();
        }
    }
}
