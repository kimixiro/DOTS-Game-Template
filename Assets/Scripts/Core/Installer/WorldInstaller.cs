using Unity.Entities;
using UnityEngine;
using Zenject;

namespace DOTSTemplate.Core.Installer
{
    public class WorldInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<World>().FromMethod(CreateWold).AsSingle().NonLazy();
        }

        private World CreateWold(InjectContext context)
        {
            var world = new World("LevelWorld");
            World.DefaultGameObjectInjectionWorld = world;

            var container = context.Container;

            var systemTypes = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default, false);
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systemTypes);

            foreach (var system in world.Systems)
            {
                container.Inject(system);
            }

            return world;
        }
    }
}