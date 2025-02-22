using Zenject;
using UnityEngine;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // ���� �������� Bind-�
        Container.Bind<ISlimeInput>().To<DefaultSlimeInput>().AsSingle();
        Container.Bind<ICameraInput>().To<DefaultCameraInput>().AsSingle();

    }

}