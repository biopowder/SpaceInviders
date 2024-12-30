using Coins;
using Difficulty;
using Modules;
using SnakeGame;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField]
        private int maxDifficulty = 5;

        [SerializeField]
        private WorldBounds worldBounds;

        [SerializeField]
        private Coin coinPrefab;

        public override void InstallBindings()
        {
            Container.Bind<IWorldBounds>().FromInstance(worldBounds).AsSingle();
            Container.BindInterfacesAndSelfTo<GameCycle.GameCycle>().AsSingle();
            Container.Bind<IDifficulty>().To<Modules.Difficulty>().AsSingle().WithArguments(maxDifficulty);
            Container.Bind<IScore>().To<Score>().AsSingle();
            Container.BindInterfacesTo<CoinSpawner>().AsSingle().WithArguments(coinPrefab);
            Container.BindInterfacesTo<DifficultyManager>().AsCached();
        }
    }
}