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
            character.OnDeath += GameOver;
        }

        private void Start()
        {
            character.Setup(bulletManager);
        }

        private static void GameOver()
        {
            Time.timeScale = 0;
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
                character.Shoot();
                _fireRequired = false;
            }

            character.Move(new Vector2(_moveDirection, 0));
        }

        private void OnDestroy()
        {
            character.OnDeath -= GameOver;
        }
    }
}