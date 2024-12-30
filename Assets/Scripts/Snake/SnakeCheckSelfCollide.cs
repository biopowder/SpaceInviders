using System;
using GameCycle;
using Modules;
using Zenject;

namespace Snake
{
    public class SnakeCheckSelfCollide : IInitializable, IDisposable
    {
        private readonly ISnake _snake;
        private readonly IGameCycle _gameCycle;

        public SnakeCheckSelfCollide(ISnake snake, IGameCycle gameCycle)
        {
            _snake = snake;
            _gameCycle = gameCycle;
        }

        public void Initialize()
        {
            _snake.OnSelfCollided += OnSelfCollided;
        }

        public void Dispose()
        {
            _snake.OnSelfCollided -= OnSelfCollided;
        }

        private void OnSelfCollided()
        {
            _gameCycle.GameOver(false);
        }
    }
}