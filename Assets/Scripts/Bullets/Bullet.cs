using System;
using Interfaces;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class Bullet : MonoBehaviour
    {
        public event Action<Bullet> OnDestroyed;

        [SerializeField]
        private new Rigidbody2D rigidbody2D;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        public int Damage { get; set; }

        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }

        public void SetVelocity(Vector2 velocity)
        {
            rigidbody2D.velocity = velocity;
        }

        public void SetLayer(int layer)
        {
            gameObject.layer = layer;
        }

        public void SetPosition(Vector2 position)
        {
            transform.position = position;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            damageable?.TakeDamage(Damage);
            
            if (damageable != null)
            {
                OnDestroyed?.Invoke(this);
            }
        }
    }
}