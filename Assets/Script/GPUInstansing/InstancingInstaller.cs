using GPUInstancingRender.GPUInstancingRender;
using UnityEngine;
using Zenject;
namespace GPUInstancingRender
{
    public class GPUInstancingInstaller : MonoInstaller
    {
        [Header("Enemy Instancing Settings")]
        public Mesh enemyCommonMesh;
        public Material goblinMaterial;
        public Material skeletonMaterial;
        public Material zombieMaterial;

        [Header("Bullet Instancing Settings")]
        public Mesh bulletCommonMesh;
        public Material fireBulletMaterial;
        public Material frostBulletMaterial;
        public Material poisonBulletMaterial;

        [Header("Tower Instancing Settings")]
        public Mesh towerCommonMesh;
        public Material towerMaterial;

        public override void InstallBindings()
        {
            // ��������� �������� ���� ������ �� ��'����� ���� �� ��������
            var groupManager = new InstancedRenderGroupManager();
            Container.Bind<InstancedRenderGroupManager>().FromInstance(groupManager).AsSingle();

            // ��������� ����� ��� ������
            var enemyGoblinGroup = CreateInstancedGroup("EnemyGoblinGroup", enemyCommonMesh, goblinMaterial, 0);
            var enemySkeletonGroup = CreateInstancedGroup("EnemySkeletonGroup", enemyCommonMesh, skeletonMaterial, 0);
            var enemyZombieGroup = CreateInstancedGroup("EnemyZombieGroup", enemyCommonMesh, zombieMaterial, 0);

            groupManager.RegisterGroup(GPUInstancingGroupType.EnemyGoblin, enemyGoblinGroup);
            groupManager.RegisterGroup(GPUInstancingGroupType.EnemySkeleton, enemySkeletonGroup);
            groupManager.RegisterGroup(GPUInstancingGroupType.EnemyZombie, enemyZombieGroup);

            // ��������� ����� ��� ����
            var bulletFireGroup = CreateInstancedGroup("BulletFireGroup", bulletCommonMesh, fireBulletMaterial, 0);
            var bulletFrostGroup = CreateInstancedGroup("BulletFrostGroup", bulletCommonMesh, frostBulletMaterial, 0);
            var bulletPoisonGroup = CreateInstancedGroup("BulletPoisonGroup", bulletCommonMesh, poisonBulletMaterial, 0);

            groupManager.RegisterGroup(GPUInstancingGroupType.BulletFire, bulletFireGroup);
            groupManager.RegisterGroup(GPUInstancingGroupType.BulletFrost, bulletFrostGroup);
            groupManager.RegisterGroup(GPUInstancingGroupType.BulletPoison, bulletPoisonGroup);

            // ��������� ����� ��� ���
            var towerGroup = CreateInstancedGroup("TowerGroup", towerCommonMesh, towerMaterial, 0);
            groupManager.RegisterGroup(GPUInstancingGroupType.Tower, towerGroup);

            // ���� �������, ����� ����� ����'����� ����� ����� �� IInstancingRenderGroup � ��������� ���������������:
            Container.Bind<IInstancingRenderGroup>().WithId(GPUInstancingGroupType.EnemyGoblin).FromInstance(enemyGoblinGroup).NonLazy();
            Container.Bind<IInstancingRenderGroup>().WithId(GPUInstancingGroupType.EnemySkeleton).FromInstance(enemySkeletonGroup).NonLazy();
            Container.Bind<IInstancingRenderGroup>().WithId(GPUInstancingGroupType.EnemyZombie).FromInstance(enemyZombieGroup).NonLazy();
            Container.Bind<IInstancingRenderGroup>().WithId(GPUInstancingGroupType.BulletFire).FromInstance(bulletFireGroup).NonLazy();
            Container.Bind<IInstancingRenderGroup>().WithId(GPUInstancingGroupType.BulletFrost).FromInstance(bulletFrostGroup).NonLazy();
            Container.Bind<IInstancingRenderGroup>().WithId(GPUInstancingGroupType.BulletPoison).FromInstance(bulletPoisonGroup).NonLazy();
            Container.Bind<IInstancingRenderGroup>().WithId(GPUInstancingGroupType.Tower).FromInstance(towerGroup).NonLazy();
        }

        private InstancedRenderGroupComponent CreateInstancedGroup(string name, Mesh commonMesh, Material commonMaterial, int layer)
        {
            // ��������� ����� GameObject � ��'�� �����
            GameObject groupGO = new GameObject(name);
            // ��� ��������, ������������ ���� �� ������� �� ����� ���������
            groupGO.transform.SetParent(this.transform);
            // ������ ��������� InstancedRenderGroupComponent
            var groupComponent = groupGO.AddComponent<InstancedRenderGroupComponent>();
            // ����������� ���� ����� �������� �����
            groupComponent.SetSettings(commonMesh, commonMaterial, layer);
            return groupComponent;
        }
    }
}