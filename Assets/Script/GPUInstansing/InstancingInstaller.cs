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
            // Створюємо менеджер груп вручну та зв'язуємо його як синглтон
            var groupManager = new InstancedRenderGroupManager();
            Container.Bind<InstancedRenderGroupManager>().FromInstance(groupManager).AsSingle();

            // Створюємо групи для ворогів
            var enemyGoblinGroup = CreateInstancedGroup("EnemyGoblinGroup", enemyCommonMesh, goblinMaterial, 0);
            var enemySkeletonGroup = CreateInstancedGroup("EnemySkeletonGroup", enemyCommonMesh, skeletonMaterial, 0);
            var enemyZombieGroup = CreateInstancedGroup("EnemyZombieGroup", enemyCommonMesh, zombieMaterial, 0);

            groupManager.RegisterGroup(GPUInstancingGroupType.EnemyGoblin, enemyGoblinGroup);
            groupManager.RegisterGroup(GPUInstancingGroupType.EnemySkeleton, enemySkeletonGroup);
            groupManager.RegisterGroup(GPUInstancingGroupType.EnemyZombie, enemyZombieGroup);

            // Створюємо групи для куль
            var bulletFireGroup = CreateInstancedGroup("BulletFireGroup", bulletCommonMesh, fireBulletMaterial, 0);
            var bulletFrostGroup = CreateInstancedGroup("BulletFrostGroup", bulletCommonMesh, frostBulletMaterial, 0);
            var bulletPoisonGroup = CreateInstancedGroup("BulletPoisonGroup", bulletCommonMesh, poisonBulletMaterial, 0);

            groupManager.RegisterGroup(GPUInstancingGroupType.BulletFire, bulletFireGroup);
            groupManager.RegisterGroup(GPUInstancingGroupType.BulletFrost, bulletFrostGroup);
            groupManager.RegisterGroup(GPUInstancingGroupType.BulletPoison, bulletPoisonGroup);

            // Створюємо групу для веж
            var towerGroup = CreateInstancedGroup("TowerGroup", towerCommonMesh, towerMaterial, 0);
            groupManager.RegisterGroup(GPUInstancingGroupType.Tower, towerGroup);

            // Якщо потрібно, можна також прив'язати кожну групу як IInstancingRenderGroup з унікальним ідентифікатором:
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
            // Створюємо новий GameObject з ім'ям групи
            GameObject groupGO = new GameObject(name);
            // Для зручності, встановлюємо його як дочірній до цього інсталера
            groupGO.transform.SetParent(this.transform);
            // Додаємо компонент InstancedRenderGroupComponent
            var groupComponent = groupGO.AddComponent<InstancedRenderGroupComponent>();
            // Налаштовуємо його через публічний метод
            groupComponent.SetSettings(commonMesh, commonMaterial, layer);
            return groupComponent;
        }
    }
}