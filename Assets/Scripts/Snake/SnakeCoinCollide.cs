using System;
using Coins;
using Modules;
using UnityEngine;
using Zenject;

namespace Snake
{
    public class SnakeCoinCollide : IInitializable, IDisposable
    {
        private readonly ISnake _snake;
        private readonly ICoinSpawner _coinSpawner;
        private readonly IScore _score;

        public SnakeCoinCollide(ISnake snake, ICoinSpawner coinSpawner, IScore score)
        {
            _snake = snake;
            _coinSpawner = coinSpawner;
            _score = score;
        }

        public void Initialize()
        {
            _snake.OnMoved += OnMoved;
        }

        public void Dispose()
        {
            _snake.OnMoved -= OnMoved;
        }

        private void OnMoved(Vector2Int obj)
        {
            if (_coinSpawner.IsCoinOnPosition(obj, out ICoin coin))
            {
                _coinSpawner.CollectCoin(coin);
                _score.Add(100);
                _snake.Expand(coin.Bones);
            }
        }
    }
}