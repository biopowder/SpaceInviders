using System.Collections.Generic;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class BulletManager : MonoBehaviour
    {
        [SerializeField]
        public Bullet prefab;

        [SerializeField]
        public Transform worldTransform;

        [SerializeField]
        private LevelBounds levelBounds;

        [SerializeField]
        private Transform container;

        private readonly Queue<Bullet> _bulletPool = new();

        private void Awake()
        {
            for (int i = 0; i < 10; i++)
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

            bullet.transform.position = position;
            bullet.spriteRenderer.color = color;
            bullet.gameObject.layer = physicsLayer;
            bullet.Damage = damage;
            bullet.rigidbody2D.velocity = velocity;
            bullet.Setup(this, levelBounds);
        }

        public void RemoveBullet(Bullet bullet)
        {
            bullet.transform.SetParent(container);
            _bulletPool.Enqueue(bullet);
        }
    }
}