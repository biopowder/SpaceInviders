using UnityEngine;

namespace ShootEmUp
{
    public class PlayerDeathObserver : MonoBehaviour
    {
        [SerializeField]
        private Ship character;

        private void Awake()
        {
            character.OnDeath += GameOver;
        }

        private static void GameOver()
        {
            Time.timeScale = 0;
        }

        private void OnDestroy()
        {
            character.OnDeath -= GameOver;
        }
    }
}