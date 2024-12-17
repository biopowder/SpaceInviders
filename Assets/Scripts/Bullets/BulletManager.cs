using System.Collections.Generic;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class BulletManager : MonoBehaviour
    {
        [SerializeField]
        private LevelBounds levelBounds;

        [SerializeField]
        private BulletPool bulletPool;

        private readonly List<Bullet> _bullets = new();

        public void SpawnBullet(
            Vector2 position,
            Color color,
            int physicsLayer,
            int damage,
            Vector2 velocity
        )
        {
            Bullet bullet = bulletPool.Get();

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
            bulletPool.Return(bullet);
            _bullets.Remove(bullet);
        }
    }
}