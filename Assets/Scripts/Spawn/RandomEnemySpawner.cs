using System;
using System.Linq;
using Enemy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spawn
{
    public class RandomEnemySpawner : EnemySpawner
    {
        [Serializable]
        public struct EnemySpawnData
        {
            public EnemyController enemy;
            public float weight;
        }

        [Header("Prefabs/Weights")]
        [Tooltip("syncs with enemySpawnTable")]
        [SerializeField] private EnemyController[] enemyPrefabs;
        [Tooltip("syncs with enemyPrefabs")]
        [SerializeField] private EnemySpawnData[] enemySpawnTable;

        [Header("Parameters")]
        [SerializeField] private int enemySpawnCount = 1;

        public override void SpawnEnemies()
        {
            var ratioTotal =
                enemySpawnTable.Aggregate(0f,
                    (i, data) => { return i += data.weight; });
            var failSafe = 0;
            for (var i = 0; i < enemySpawnCount; i++)
            {
                if (failSafe >= 200)
                {
                    Debug.LogError("failed to spawn at");
                    break;
                }

                var roll = Random.Range(0f, ratioTotal);
                var enemy = enemySpawnTable
                    .Single(data => (roll -= data.weight) < 0)
                    .enemy;
                SpawnEnemy(enemy, out var result);
                if (!result)
                {
                    i--;
                    failSafe++;
                }
            }
        }

        private void OnValidate()
        {
            if (enemyPrefabs == null || enemyPrefabs.Length == 0)
            {
                if (enemySpawnTable != null && enemySpawnTable.Length > 0)
                {
                    enemyPrefabs = enemySpawnTable.Where(data => data.enemy != null)
                        .Select(data => data.enemy).ToArray();
                }
            }
            else if (enemySpawnTable == null || enemySpawnTable.Length == 0)
            {
                if (enemyPrefabs.Length > 0)
                {
                    enemySpawnTable = enemyPrefabs.Select(controller => new
                        EnemySpawnData {enemy = controller}).ToArray();
                }
            }
            else if (!enemyPrefabs.SequenceEqual(
                enemySpawnTable.Select(data => data.enemy)))
            {
                enemyPrefabs = enemySpawnTable.Where(data => data.enemy != null)
                    .Select(data => data.enemy).ToArray();
            }
        }
    }
}