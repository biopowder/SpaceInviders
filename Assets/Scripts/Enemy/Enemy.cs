using UnityEngine;

namespace ShootEmUp
{
    public sealed class Enemy : Ship
    {
        [SerializeField]
        private float countdown;

        public bool IsDead => CurrentHealth <= 0;

        private Rigidbody2D _rigidbody;

        private Vector2 _destination;
        private float _currentTime;
        private bool _isPointReached;

        private Player _target;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        public override void Move(Vector2 direction)
        {
            if (direction.magnitude <= 0.25f)
            {
                _isPointReached = true;
                return;
            }

            direction = direction.normalized * Time.fixedDeltaTime;
            Vector2 nextPosition = _rigidbody.position + direction * Speed;
            _rigidbody.MovePosition(nextPosition);
        }

        public override void Shoot()
        {
            Vector2 startPosition = firePoint.position;
            Vector2 vector = (Vector2)_target.transform.position - startPosition;
            Vector2 direction = vector.normalized;

            BulletManager.SpawnBullet(startPosition,
                Color.red,
                (int)PhysicsLayer.EnemyBullet,
                damage,
                direction);
        }

        private void OnEnable()
        {
            CurrentHealth = maxHealth;
        }

        public void SetPlayer(Player player)
        {
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

                Shoot();

                _currentTime += countdown;
            }
            else
            {
                Vector2 vector = _destination - (Vector2)transform.position;
                Move(vector);
            }
        }
    }
}