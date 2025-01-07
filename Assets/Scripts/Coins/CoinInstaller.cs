using Modules;
using Zenject;

namespace Coins
{
    public class CoinInstaller : Installer<Coin, CoinInstaller>
    {
        [Inject]
        private Coin _coinPrefab;
        
        public override void InstallBindings()
        {
            Container.BindMemoryPool<Coin, CoinPool>()
                .WithInitialSize(5)
                .FromComponentInNewPrefab(_coinPrefab)
                .WithGameObjectName("Coin")
                .UnderTransformGroup("Coins");

            // Container.BindInterfacesTo<CoinSpawner>().AsSingle();
            Container.Bind<ICoinSpawner>().To<CoinSpawner>().AsSingle();
        }
    }
}