using UnityEngine;

namespace ShootEmUp
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Player character;

        [SerializeField]
        private BulletManager bulletManager;

        private bool fireRequired;
        private float moveDirection;

        private void Awake()
        {
            character.OnHealthEmpty += _ => Time.timeScale = 0;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) 
                fireRequired = true;

            if (Input.GetKey(KeyCode.LeftArrow))
                moveDirection = -1;
            else if (Input.GetKey(KeyCode.RightArrow))
                moveDirection = 1;
            else
                moveDirection = 0;
        }

        private void FixedUpdate()
        {
            if (fireRequired)
            {
                bulletManager.SpawnBullet(
                    character.firePoint.position,
                    Color.blue,
                    (int) PhysicsLayer.PLAYER_BULLET,
                    1,
                    true,
                    character.firePoint.rotation * Vector3.up * 3
                );

                fireRequired = false;
            }
            
            Vector2 moveDirection = new Vector2(this.moveDirection, 0);
            Vector2 moveStep = moveDirection * Time.fixedDeltaTime * character.speed;
            Vector2 targetPosition = character._rigidbody.position + moveStep;
            character._rigidbody.MovePosition(targetPosition);
        }
    }
}