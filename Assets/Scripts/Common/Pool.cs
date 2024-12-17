using System.Collections.Generic;
using UnityEngine;

namespace ShootEmUp
{
    public class Pool<T> : MonoBehaviour where T : Component
    {
        [SerializeField]
        private int initialPoolSize = 5;

        [SerializeField]
        private Factory<T> factory;

        private readonly Queue<T> _itemPool = new();

        private void Awake()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                T item = factory.Create();
                item.gameObject.SetActive(false);
                _itemPool.Enqueue(item);
            }
        }

        public T Get()
        {
            if (!_itemPool.TryDequeue(out T item))
            {
                item = factory.Create();
            }

            item.gameObject.SetActive(true);
            return item;
        }

        public void Return(T item)
        {
            item.gameObject.SetActive(false);
            _itemPool.Enqueue(item);
        }
    }
}