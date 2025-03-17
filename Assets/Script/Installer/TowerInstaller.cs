using UnityEngine;
using Zenject;

namespace WannaBe
{
    public class TowerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // Знаходимо всі компоненти Tower у ієрархії сцени та реєструємо їх як кешовані (ас-кешед)
            Container.Bind<Tower>().FromComponentsInHierarchy().AsCached();
        }
    }
}
