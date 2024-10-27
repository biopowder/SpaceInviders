using Interfaces;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class Enemy : MonoBehaviour, IDamageable
    {
        [SerializeField]
        public Transform firePoint;

        [SerializeField]
        private int health;

        [SerializeField]
        public float speed = 5.0f;

        [SerializeField]
        private float countdown;

        [SerializeField]
        private int damage = 1;

        public bool IsDead => _currentHealth <= 0;

        private int _currentHealth;
        private Rigidbody2D _rigidbody;

        private Vector2 _destination;
        private float _currentTime;
        private bool _isPointReached;

        private BulletManager _bulletManager;
        private Player _target;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            _currentHealth = health;
        }

        public void Setup(BulletManager bulletManager, Player player)
        {
            _bulletManager = bulletManager;
            _target = player;
        }

        public void Reset()
        {
            _currentTime = countdown;
        }

        public void SetDestination(Vector2 endPoint)
        {
            _destination = endPoint;
            _isPointReached = false;
        }

        private void FixedUpdate()
        {
            if (_isPointReached)
            {
                if (!_target.IsAlive) return;

                _currentTime -= Time.fixedDeltaTime;

                if (_currentTime > 0) return;

                Vector2 startPosition = firePoint.position;
                Vector2 vector = (Vector2)_target.transform.position - startPosition;
                Vector2 direction = vector.normalized;

                _bulletManager.SpawnBullet(startPosition,
                    Color.red,
                    (int)PhysicsLayer.EnemyBullet,
                    damage,
                    direction);

                _currentTime += countdown;
            }
            else
            {
                Vector2 vector = _destination - (Vector2)transform.position;
                if (vector.magnitude <= 0.25f)
                {
                    _isPointReached = true;
                    return;
                }

                Vector2 direction = vector.normalized * Time.fixedDeltaTime;
                Vector2 nextPosition = _rigidbody.position + direction * speed;
                _rigidbody.MovePosition(nextPosition);
            }
        }

        public void TakeDamage(int damageTaken)
        {
            _currentHealth -= damageTaken;
        }
    }
}