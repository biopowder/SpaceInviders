using UnityEngine;

namespace ShootEmUp
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Player character;

        [SerializeField]
        private BulletManager bulletManager;

        private bool _fireRequired;
        private float _moveDirection;

        private void Awake()
        {
            // character.OnHealthEmpty += _ => Time.timeScale = 0;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _fireRequired = true;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _moveDirection = -1;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                _moveDirection = 1;
            }
            else
            {
                _moveDirection = 0;
            }
        }

        private void FixedUpdate()
        {
            if (_fireRequired)
            {
                bulletManager.SpawnBullet(
                    character.firePoint.position,
                    Color.blue,
                    (int)PhysicsLayer.PlayerBullet,
                    1,
                    character.firePoint.rotation * Vector3.up * 3
                );

                _fireRequired = false;
            }

            Vector2 moveDirection = new(_moveDirection, 0);
            Vector2 moveStep = moveDirection * (Time.fixedDeltaTime * character.Speed);
            Vector2 targetPosition = character.Rigidbody.position + moveStep;
            character.Rigidbody.MovePosition(targetPosition);
        }
    }
}