using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootEmUp
{
    public sealed class EnemyManager : MonoBehaviour
    {
        [SerializeField]
        private Transform[] spawnPositions;

        [SerializeField]
        private Transform[] attackPositions;

        [SerializeField]
        private Ship character;

        [SerializeField]
        private BulletManager bulletSystem;

        [SerializeField]
        private EnemyPool enemyPool;

        private readonly HashSet<Ship> _mActiveEnemies = new();

        public void Spawn()
        {
            Ship ship = enemyPool.Get();

            Transform spawnPosition = RandomPoint(spawnPositions);
            ship.transform.position = spawnPosition.position;

            Transform attackPosition = RandomPoint(attackPositions);

            EnemyController enemyController = ship.GetComponent<EnemyController>();
            enemyController.SetDestination(attackPosition.position);
            enemyController.SetPlayer(character);

            ship.Setup(bulletSystem);

            _mActiveEnemies.Add(ship);
        }

        private void FixedUpdate()
        {
            foreach (Ship enemy in _mActiveEnemies.ToArray())
            {
                if (enemy.IsAlive) continue;

                _mActiveEnemies.Remove(enemy);
                enemyPool.Return(enemy);
            }
        }

        private static Transform RandomPoint(Transform[] points)
        {
            int index = Random.Range(0, points.Length);
            return points[index];
        }
    }
}