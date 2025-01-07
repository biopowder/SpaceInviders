using Modules;
using Zenject;

namespace Difficulty
{
    public class DifficultyInstaller : Installer<int, DifficultyInstaller>
    {
        [Inject]
        private int _maxDifficulty;

        public override void InstallBindings()
        {
            Container.Bind<IDifficulty>().To<Modules.Difficulty>().AsSingle().WithArguments(_maxDifficulty);
            Container.BindInterfacesTo<DifficultyManager>().AsCached();
        }
    }
}