using System.Collections;
using UnityEngine;

namespace ShootEmUp
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private float minSpawnTime = 1;

        [SerializeField]
        private float maxSpawnTime = 3;

        [SerializeField]
        private EnemyManager enemyManager;

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
                enemyManager.Spawn();
            }
        }
    }
}