using System;
using Modules;
using UnityEngine;

namespace Coins
{
    public interface ICoinSpawner
    {
        event Action OnCollectAllCoins;
        bool IsCoinOnPosition(Vector2Int position, out Coin coin);
        void SpawnCoins(int count);
        void CollectCoin(Coin coin);
    }
}