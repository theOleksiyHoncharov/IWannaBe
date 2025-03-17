using UnityEngine;
using Zenject;

namespace WannaBe
{
    public class TowerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // ��������� �� ���������� Tower � �������� ����� �� �������� �� �� ������� (��-�����)
            Container.Bind<Tower>().FromComponentsInHierarchy().AsCached();
        }
    }
}
