using System.Collections.Generic;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class BulletManager : MonoBehaviour
    {
        [SerializeField]
        private Bullet prefab;

        [SerializeField]
        private Transform worldTransform;

        [SerializeField]
        private LevelBounds levelBounds;

        [SerializeField]
        private Transform container;

        [SerializeField]
        private int initialBulletCountPool = 10;

        private readonly Queue<Bullet> _bulletPool = new();

        private readonly List<Bullet> _bullets = new();

        private void Awake()
        {
            for (int i = 0; i < initialBulletCountPool; i++)
            {
                Bullet bullet = Instantiate(prefab, container);
                _bulletPool.Enqueue(bullet);
            }
        }

        public void SpawnBullet(
            Vector2 position,
            Color color,
            int physicsLayer,
            int damage,
            Vector2 velocity
        )
        {
            if (_bulletPool.TryDequeue(out Bullet bullet))
            {
                bullet.transform.SetParent(worldTransform);
            }
            else
            {
                bullet = Instantiate(prefab, worldTransform);
            }

            bullet.SetPosition(position);
            bullet.SetLayer(physicsLayer);
            bullet.SetColor(color);
            bullet.SetVelocity(velocity);
            bullet.Damage = damage;
            bullet.OnDestroyed += RemoveBullet;

            _bullets.Add(bullet);
        }

        private void FixedUpdate()
        {
            for (int i = _bullets.Count - 1; i >= 0; i--)
            {
                if (!levelBounds.InBounds(_bullets[i].transform.position))
                {
                    RemoveBullet(_bullets[i]);
                }
            }
        }

        private void RemoveBullet(Bullet bullet)
        {
            bullet.transform.SetParent(container);
            _bulletPool.Enqueue(bullet);
            _bullets.Remove(bullet);
        }
    }
}