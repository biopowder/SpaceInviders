using UnityEngine;

namespace ShootEmUp
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Ship character;

        private bool _fireRequired;
        private float _moveDirection;

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
                character.Shoot(Vector3.up * 3);
                _fireRequired = false;
            }

            character.Move(new Vector2(_moveDirection, 0));
        }
    }
}