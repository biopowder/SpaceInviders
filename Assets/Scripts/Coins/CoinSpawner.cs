using System;
using System.Collections.Generic;
using Modules;
using SnakeGame;
using UnityEngine;

namespace Coins
{
    public class CoinSpawner : ICoinSpawner
    {
        public event Action OnCollectAllCoins;

        private readonly IWorldBounds _worldBounds;
        private readonly CoinPool _coinPool;
        private readonly List<Coin> _activeCoins = new();

        private readonly Action _onGameStart;

        private readonly Dictionary<ICoin, GameObject> _coinObjects = new();

        public CoinSpawner(IWorldBounds worldBounds, CoinPool coinPool)
        {
            _worldBounds = worldBounds;
            _coinPool = coinPool;
        }

        public bool IsCoinOnPosition(Vector2Int position, out Coin coin)
        {
            foreach (Coin activeCoin in _activeCoins)
            {
                if (activeCoin.Position == position)
                {
                    coin = activeCoin;
                    return true;
                }
            }

            coin = null;
            return false;
        }

        public void SpawnCoins(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2Int position;
                do
                {
                    position = _worldBounds.GetRandomPosition();
                } while (IsPositionOccupied(position));

                Coin coin = _coinPool.Spawn(position);
                coin.Generate();
                _activeCoins.Add(coin);
                _coinObjects[coin] = coin.gameObject;
            }
        }

        private bool IsPositionOccupied(Vector2Int position)
        {
            foreach (ICoin coin in _activeCoins)
            {
                if (coin.Position == position)
                {
                    return true;
                }
            }

            return false;
        }

        public void CollectCoin(Coin coin)
        {
            _coinPool.Despawn(coin);
            _activeCoins.Remove(coin);

            if (_activeCoins.Count == 0)
            {
                OnCollectAllCoins?.Invoke();
            }
        }
    }
}