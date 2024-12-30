using System;
using System.Numerics;
using GameCycle;
using Modules;
using SnakeGame;
using UnityEngine;
using Zenject;

namespace Snake
{
    public class SnakeOutOfWorldBounds : IInitializable, IDisposable
    {
        private readonly ISnake _snake;
        private readonly IWorldBounds _worldBounds;
        private readonly IGameCycle _gameCycle;

        public SnakeOutOfWorldBounds(ISnake snake, IWorldBounds worldBounds, IGameCycle gameCycle)
        {
            _snake = snake;
            _worldBounds = worldBounds;
            _gameCycle = gameCycle;
        }
        
        public void Initialize()
        {
            _snake.OnMoved += OnMoved;
        }

        public void Dispose()
        {
            _snake.OnMoved -= OnMoved;
        }

        private void OnMoved(Vector2Int newPosition)
        {
            if (!_worldBounds.IsInBounds(newPosition))
            {
                _gameCycle.GameOver(false);
            }
        }
    }
}