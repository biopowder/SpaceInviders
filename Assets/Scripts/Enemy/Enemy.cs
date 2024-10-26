using System;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class Enemy : MonoBehaviour
    {
        public delegate void FireHandler(Vector2 position, Vector2 direction);
        
        public event FireHandler OnFire;

        [SerializeField]
        public bool isPlayer;
        
        [SerializeField]
        public Transform firePoint;
        
        [SerializeField]
        public int health;

        [SerializeField]
        public Rigidbody2D _rigidbody;

        [SerializeField]
        public float speed = 5.0f;

        [SerializeField]
        private float countdown;

        [NonSerialized]
        public Player target;

        private Vector2 destination;
        private float currentTime;
        private bool isPointReached;

        public void Reset()
        {
            currentTime = countdown;
        }
        
        public void SetDestination(Vector2 endPoint)
        {
            destination = endPoint;
            isPointReached = false;
        }

        private void FixedUpdate()
        {
            if (isPointReached)
            {
                //Attack:
                if (target.health <= 0)
                    return;

                currentTime -= Time.fixedDeltaTime;
                if (currentTime <= 0)
                {
                    Vector2 startPosition = firePoint.position;
                    Vector2 vector = (Vector2) target.transform.position - startPosition;
                    Vector2 direction = vector.normalized;
                    OnFire?.Invoke(startPosition, direction);
                    
                    currentTime += countdown;
                }
            }
            else
            {
                //Move:
                Vector2 vector = destination - (Vector2) transform.position;
                if (vector.magnitude <= 0.25f)
                {
                    isPointReached = true;
                    return;
                }

                Vector2 direction = vector.normalized * Time.fixedDeltaTime;
                Vector2 nextPosition = _rigidbody.position + direction * speed;
                _rigidbody.MovePosition(nextPosition);
            }
        }
    }
}