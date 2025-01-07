using SnakeGame;
using Zenject;

namespace UI
{
    public class GameUiInstaller : Installer<GameUiInstaller>
    {
        public override void InstallBindings()
        {
            Container.Bind<GameUI>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesTo<GameUi>().AsCached();
        }
    }
}