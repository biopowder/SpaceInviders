using System;
using Interfaces;
using UnityEngine;

namespace ShootEmUp
{
    public class Ship : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private Transform firePoint;

        [SerializeField]
        private int maxHealth;

        [SerializeField]
        private float speed = 5.0f;

        [SerializeField]
        private int damage = 1;

        [SerializeField]
        private int layer;

        [SerializeField]
        private Color bulletColor;

        public bool IsAlive => _currentHealth > 0;

        private Rigidbody2D _playerRigidbody;

        private int _currentHealth;

        private BulletManager _bulletManager;

        public event Action OnDeath;

        private void Awake()
        {
            _playerRigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            _currentHealth = maxHealth;
        }

        public void Setup(BulletManager bulletManager)
        {
            _bulletManager = bulletManager;
            _currentHealth = maxHealth;
        }

        public void Move(Vector2 direction)
        {
            Vector2 moveStep = direction * (Time.fixedDeltaTime * speed);
            Vector2 targetPosition = _playerRigidbody.position + moveStep;
            _playerRigidbody.MovePosition(targetPosition);
        }

        public void Shoot(Vector3 direction)
        {
            Vector2 startPosition = firePoint.position;

            _bulletManager.SpawnBullet(startPosition,
                bulletColor,
                layer,
                damage,
                direction);
        }

        public virtual void TakeDamage(int damageToTake)
        {
            _currentHealth -= damageToTake;

            if (_currentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }
    }
}