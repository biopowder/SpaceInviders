using System;
using UnityEngine;

namespace GameCycle
{
    public class GameCycle : IGameCycle
    {
        public event Action OnGameStart;
        public event Action<bool> OnGameOver;

        public void StartGame()
        {
            OnGameStart?.Invoke();
        }

        public void GameOver(bool win)
        {
            Time.timeScale = 0;
            OnGameOver?.Invoke(win);
        }
    }
}