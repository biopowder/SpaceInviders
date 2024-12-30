using System;
using Modules;
using SnakeGame;
using Zenject;

namespace UI
{
    public class GameUi : IInitializable, IDisposable
    {
        private readonly GameUI _gameUI;
        private readonly GameCycle.GameCycle _gameCycle;
        private readonly IDifficulty _difficulty;
        private readonly IScore _score;

        public GameUi(GameUI gameUI, GameCycle.GameCycle gameCycle, IDifficulty difficulty, IScore score)
        {
            _gameUI = gameUI;
            _gameCycle = gameCycle;
            _difficulty = difficulty;
            _score = score;
        }

        public void Initialize()
        {
            _difficulty.OnStateChanged += SetDifficulty;
            _score.OnStateChanged += SetScore;
            _gameCycle.OnGameOver += _gameUI.GameOver;

            SetDifficulty();
            SetScore(_score.Current);
        }

        public void Dispose()
        {
            _difficulty.OnStateChanged -= SetDifficulty;
            _score.OnStateChanged -= SetScore;
            _gameCycle.OnGameOver -= _gameUI.GameOver;
        }

        private void SetScore(int newScore)
        {
            _gameUI.SetScore(newScore.ToString());
        }

        private void SetDifficulty()
        {
            _gameUI.SetDifficulty(_difficulty.Current, _difficulty.Max);
        }
    }
}