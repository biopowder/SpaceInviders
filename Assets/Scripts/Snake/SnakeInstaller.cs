using Modules;
using Player;
using Zenject;

namespace Snake
{
    public class SnakeInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ISnake>().FromComponentInHierarchy().AsSingle();
            Container.Bind<IPlayerInput>().To<PlayerInput>().AsSingle();
            Container.BindInterfacesTo<SnakeController>().AsCached();
            Container.BindInterfacesTo<SnakeOutOfWorldBounds>().AsCached();
            Container.BindInterfacesTo<SnakeCheckSelfCollide>().AsCached();
            Container.BindInterfacesTo<SnakeCoinCollide>().AsCached();
        }
    }
}