using Modules;
using UnityEngine;

namespace Player
{
    public class PlayerInput : IPlayerInput
    {
        public SnakeDirection GetDirection()
        {
            if (Input.GetKey(KeyCode.UpArrow)) return SnakeDirection.UP;
            if (Input.GetKey(KeyCode.RightArrow)) return SnakeDirection.RIGHT;
            if (Input.GetKey(KeyCode.DownArrow)) return SnakeDirection.DOWN;
            if (Input.GetKey(KeyCode.LeftArrow)) return SnakeDirection.LEFT;
            return SnakeDirection.NONE;
        }
    }
}