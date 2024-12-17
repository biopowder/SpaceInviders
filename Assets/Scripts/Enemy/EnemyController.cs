using UnityEngine;

namespace ShootEmUp
{
    public sealed class EnemyController : MonoBehaviour
    {
        [SerializeField]
        private float countdown;

        [SerializeField]
        private Ship ship;

        private Vector2 _destination;
        private float _currentTime;
        private bool _isPointReached;

        private Ship _target;

        private void Move(Vector2 direction)
        {
            if (direction.magnitude <= 0.25f)
            {
                _isPointReached = true;
                return;
            }

            ship.Move(direction);
        }

        public void SetPlayer(Ship player)
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

                Vector2 startPosition = transform.position;
                Vector2 vector = (Vector2)_target.transform.position - startPosition;
                Vector2 direction = vector.normalized;

                ship.Shoot(direction);

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