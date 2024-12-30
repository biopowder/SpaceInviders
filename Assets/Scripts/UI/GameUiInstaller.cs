using SnakeGame;
using Zenject;

namespace UI
{
    public class GameUiInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameUI>().FromComponentInHierarchy().AsSingle();
            Container.BindInterfacesTo<GameUi>().AsCached();
        }
    }
}