using Coins;
using Difficulty;
using Modules;
using Snake;
using SnakeGame;
using UI;
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
            DifficultyInstaller.Install(Container, maxDifficulty);
            SnakeInstaller.Install(Container);
            GameUiInstaller.Install(Container);
            CoinInstaller.Install(Container, coinPrefab);

            Container.BindInterfacesAndSelfTo<GameCycle.GameCycle>().AsSingle();
            Container.Bind<IWorldBounds>().FromInstance(worldBounds).AsSingle();
            Container.Bind<IScore>().To<Score>().AsSingle();
        }
    }
}