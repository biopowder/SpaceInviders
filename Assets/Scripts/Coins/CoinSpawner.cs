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
        private readonly List<ICoin> _activeCoins = new();

        private readonly Coin _coinPrefab;

        private readonly Action _onGameStart;

        private readonly Dictionary<ICoin, GameObject> _coinObjects = new();

        public CoinSpawner(IWorldBounds worldBounds, Coin coinPrefab)
        {
            _worldBounds = worldBounds;
            _coinPrefab = coinPrefab;
        }

        public bool IsCoinOnPosition(Vector2Int position, out ICoin coin)
        {
            foreach (ICoin activeCoin in _activeCoins)
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

                Coin coin = GameObject.Instantiate(_coinPrefab, (Vector2)position, Quaternion.identity, null);
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

        public void CollectCoin(ICoin coin)
        {
            GameObject.Destroy(_coinObjects[coin]);
            _activeCoins.Remove(coin);

            if (_activeCoins.Count == 0)
            {
                OnCollectAllCoins?.Invoke();
            }
        }
    }
}