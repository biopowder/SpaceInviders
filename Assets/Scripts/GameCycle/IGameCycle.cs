using System;

namespace GameCycle
{
    public interface IGameCycle
    {
        event Action OnGameStart;
        event Action<bool> OnGameOver;

        void StartGame();
        void GameOver(bool win);
    }
}