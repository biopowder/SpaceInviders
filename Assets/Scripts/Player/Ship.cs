using System;
using Interfaces;
using UnityEngine;

namespace ShootEmUp
{
    public abstract class Ship : MonoBehaviour, IDamageable
    {
        [SerializeField]
        protected Transform firePoint;

        [SerializeField]
        protected int maxHealth;

        [SerializeField]
        private float speed = 5.0f;

        [SerializeField]
        protected int damage = 1;
        
        public float Speed => speed;

        public Rigidbody2D Rigidbody => _playerRigidbody;

        public bool IsAlive => CurrentHealth > 0;

        private Rigidbody2D _playerRigidbody;

        protected int CurrentHealth;
        
        protected BulletManager BulletManager;

        private void Awake()
        {
            _playerRigidbody = GetComponent<Rigidbody2D>();
        }

        public void Setup(BulletManager bulletManager)
        {
            BulletManager = bulletManager;
            CurrentHealth = maxHealth;
        }

        public abstract void Move(Vector2 direction);
        public abstract void Shoot();

        public virtual void TakeDamage(int damageToTake)
        {
            CurrentHealth -= damageToTake;
            Debug.Log($"Current health: {CurrentHealth}");
        }
    }
}