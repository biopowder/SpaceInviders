using GameCycle;
using UnityEngine;
using Zenject;

namespace DefaultNamespace
{
    public class Launcher : MonoBehaviour
    {
        [Inject]
        private readonly IGameCycle _gameCycle;

        private void Start()
        {
            _gameCycle.StartGame();
        }
    }
}