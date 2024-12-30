using System;
using Coins;
using GameCycle;
using Modules;
using Zenject;

namespace Difficulty
{
    public class DifficultyManager : IInitializable, IDisposable
    {
        private readonly IDifficulty _difficulty;
        private readonly IGameCycle _gameCycle;
        private readonly ICoinSpawner _coinSpawner;
        private readonly ISnake _snake;

        public DifficultyManager(IDifficulty difficulty, IGameCycle gameCycle, ICoinSpawner coinSpawner, ISnake snake)
        {
            _difficulty = difficulty;
            _gameCycle = gameCycle;
            _coinSpawner = coinSpawner;
            _snake = snake;
        }

        public void Initialize()
        {
            _gameCycle.OnGameStart += DifficultyUp;
            _coinSpawner.OnCollectAllCoins += DifficultyUp;
        }

        public void Dispose()
        {
            _gameCycle.OnGameStart -= DifficultyUp;
            _coinSpawner.OnCollectAllCoins -= DifficultyUp;
        }

        private void DifficultyUp()
        {
            if (_difficulty.Next(out int difficulty))
            {
                _coinSpawner.SpawnCoins(difficulty);
                _snake.SetSpeed(difficulty);
            }
            else
            {
                _gameCycle.GameOver(win: true);
            }
        }
    }
}