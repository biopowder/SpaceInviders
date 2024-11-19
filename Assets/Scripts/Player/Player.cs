using System;
using UnityEngine;

namespace ShootEmUp
{
    public class Player : Ship
    {
        public event Action OnDeath;
        
        public override void Move(Vector2 direction)
        {
            Vector2 moveStep = direction * (Time.fixedDeltaTime * Speed);
            Vector2 targetPosition = Rigidbody.position + moveStep;
            Rigidbody.MovePosition(targetPosition);
        }

        public override void Shoot()
        {
            BulletManager.SpawnBullet(
                firePoint.position,
                Color.blue,
                (int)PhysicsLayer.PlayerBullet,
                damage,
                firePoint.rotation * Vector3.up * 3
            );
        }

        public override void TakeDamage(int damageToTake)
        {
            base.TakeDamage(damageToTake);

            if (CurrentHealth <= 0)
            {
                OnDeath?.Invoke();
            }
        }
    }
}