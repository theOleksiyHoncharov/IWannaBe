using Zenject;
using UnityEngine;

namespace WannaBe
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Ваші глобальні Bind-и
            Container.Bind<ISlimeInput>().To<DefaultSlimeInput>().AsSingle();
            Container.Bind<ICameraInput>().To<DefaultCameraInput>().AsSingle();
        }

    }
}