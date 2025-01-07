using Modules;
using UnityEngine;
using Zenject;

namespace Coins
{
    public sealed class CoinPool : MonoMemoryPool<Vector2Int, Coin>
    {
        protected override void Reinitialize(Vector2Int position, Coin item)
        {
            item.transform.position = new Vector3(position.x, position.y, 0);
        }
    }
}