using DOTSTemplate;
using UnityEngine;
using Zenject;

namespace DOTSTemplate
{
    public class ProjectInstaller: MonoInstaller
    {
        [SerializeField] private Database _database;
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<InputServices>()
                .AsSingle()
                .Lazy();
            Container.BindInterfacesTo<Database>()
                .FromInstance(_database)
                .AsSingle();
            Container.BindInterfacesTo<GameService>()
                .AsSingle()
                .NonLazy();
        }
    }
}