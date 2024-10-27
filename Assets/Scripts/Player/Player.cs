using Interfaces;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class Player : MonoBehaviour, IDamageable
    {
        [SerializeField]
        public Transform firePoint;

        [SerializeField]
        private int maxHealth;

        [SerializeField]
        private float speed = 5.0f;

        public float Speed => speed;

        public Rigidbody2D Rigidbody => _playerRigidbody;

        public bool IsAlive => _currentHealth > 0;

        private Rigidbody2D _playerRigidbody;

        private int _currentHealth;

        private void Awake()
        {
            _currentHealth = maxHealth;
            _playerRigidbody = GetComponent<Rigidbody2D>();
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                // Notify
            }
        }
    }
}