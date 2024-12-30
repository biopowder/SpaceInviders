using Modules;
using Player;
using Zenject;

namespace Snake
{
    public class SnakeController : ITickable
    {
        private readonly ISnake _snake;
        private readonly IPlayerInput _input;

        public SnakeController(ISnake snake, IPlayerInput input)
        {
            _snake = snake;
            _input = input;
        }

        public void Tick()
        {
            SnakeDirection direction = _input.GetDirection();

            // I would add here: if _snake.Direction != direction
            if (direction != SnakeDirection.NONE)
            {
                _snake.Turn(direction);
            }
        }
    }
}