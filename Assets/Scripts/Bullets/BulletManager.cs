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

        public readonly HashSet<Bullet> MActiveBullets = new();
        public readonly Queue<Bullet> MBulletPool = new();
        private readonly List<Bullet> mCache = new();

        private void Awake()
        {
            for (int i = 0; i < 10; i++)
            {
                Bullet bullet = Instantiate(prefab, container);
                MBulletPool.Enqueue(bullet);
            }
        }

        private void FixedUpdate()
        {
            mCache.Clear();
            mCache.AddRange(MActiveBullets);

            for (int i = 0, count = mCache.Count; i < count; i++)
            {
                Bullet bullet = mCache[i];
                if (!levelBounds.InBounds(bullet.transform.position))
                {
                    RemoveBullet(bullet);
                }
            }
        }

        public void SpawnBullet(
            Vector2 position,
            Color color,
            int physicsLayer,
            int damage,
            bool isPlayer,
            Vector2 velocity
        )
        {
            if (MBulletPool.TryDequeue(out Bullet bullet))
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
            bullet.damage = damage;
            bullet.isPlayer = isPlayer;
            bullet.rigidbody2D.velocity = velocity;

            if (MActiveBullets.Add(bullet))
            {
                bullet.OnCollisionEntered += OnBulletCollision;
            }
        }

        private void OnBulletCollision(Bullet bullet, Collision2D collision)
        {
            DealDamage(bullet, collision.gameObject);
            RemoveBullet(bullet);
        }

        private void RemoveBullet(Bullet bullet)
        {
            if (MActiveBullets.Remove(bullet))
            {
                bullet.OnCollisionEntered -= OnBulletCollision;
                bullet.transform.SetParent(container);
                MBulletPool.Enqueue(bullet);
            }
        }

        private void DealDamage(Bullet bullet, GameObject other)
        {
            int damage = bullet.damage;
            if (damage <= 0)
                return;
            
            if (other.TryGetComponent(out Player player))
            {
                if (bullet.isPlayer != player.isPlayer)
                {
                    if (player.health <= 0)
                        return;

                    player.health = Mathf.Max(0, player.health - damage);
                    player.OnHealthChanged?.Invoke(player, player.health);

                    if (player.health <= 0)
                        player.OnHealthEmpty?.Invoke(player);
                }
            }
            else if (other.TryGetComponent(out Enemy enemy))
            {
                if (bullet.isPlayer != enemy.isPlayer)
                {
                    if (enemy.health > 0)
                    {
                        enemy.health = Mathf.Max(0, enemy.health - damage);
                    }
                }
            }
        }
    }
}