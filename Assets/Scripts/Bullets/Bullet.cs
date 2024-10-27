using System;
using Interfaces;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class Bullet : MonoBehaviour
    {
        [NonSerialized]
        public int Damage;

        [SerializeField]
        public new Rigidbody2D rigidbody2D;

        [SerializeField]
        public SpriteRenderer spriteRenderer;

        private BulletManager _bulletManager;
        private LevelBounds _levelBounds;

        public void Setup(BulletManager bulletManager, LevelBounds levelBounds)
        {
            _bulletManager = bulletManager;
            _levelBounds = levelBounds;
        }

        private void FixedUpdate()
        {
            if (!_levelBounds.InBounds(transform.position))
            {
                _bulletManager.RemoveBullet(this);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            damageable?.TakeDamage(Damage);

            _bulletManager.RemoveBullet(this);
        }
    }
}