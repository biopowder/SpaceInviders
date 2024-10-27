using System.Collections;
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
        private Player character;

        [SerializeField]
        private Transform worldTransform;

        [SerializeField]
        private Transform container;

        [SerializeField]
        private Enemy prefab;

        [SerializeField]
        private BulletManager bulletSystem;

        private readonly HashSet<Enemy> _mActiveEnemies = new();
        private readonly Queue<Enemy> _enemyPool = new();

        private void Awake()
        {
            for (var i = 0; i < 7; i++)
            {
                Enemy enemy = Instantiate(prefab, container);
                _enemyPool.Enqueue(enemy);
            }
        }

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(1, 2));

                if (!_enemyPool.TryDequeue(out Enemy enemy))
                {
                    enemy = Instantiate(prefab, container);
                }

                enemy.transform.SetParent(worldTransform);

                Transform spawnPosition = RandomPoint(spawnPositions);
                enemy.transform.position = spawnPosition.position;

                Transform attackPosition = RandomPoint(attackPositions);
                enemy.SetDestination(attackPosition.position);
                enemy.Setup(bulletSystem, character);

                _mActiveEnemies.Add(enemy);
            }
        }

        private void FixedUpdate()
        {
            foreach (Enemy enemy in _mActiveEnemies.ToArray())
            {
                if (!enemy.IsDead) continue;

                enemy.transform.SetParent(container);

                _mActiveEnemies.Remove(enemy);
                _enemyPool.Enqueue(enemy);
            }
        }

        private static Transform RandomPoint(Transform[] points)
        {
            int index = Random.Range(0, points.Length);
            return points[index];
        }
    }
}