using System;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using RoomLoading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spawn
{
    [RequireComponent(typeof(RoomData))]
    public abstract class EnemySpawner : MonoBehaviour
    {
        private GameObject _enemyChild;
        private readonly List<EnemyController> _spawnedEnemies =
            new List<EnemyController>();

        private RoomData _roomData;
        protected RoomData RoomData
        {
            get
            {
                if (!_roomData)
                {
                    _roomData = GetComponent<RoomData>();
                }

                return _roomData;
            }
        }
        public abstract void SpawnEnemies();

        private Vector3Int GetRandomEmptyCell(out bool result)
        {
            //Update Bounds, no calculation is done if already performed on room
            RoomData.GetRoomHeight();
            var bounds = RoomData.tilemap.cellBounds;
            Vector3Int randomPos;
            var failSafe = 0;
            do
            {
                randomPos = new Vector3Int
                {
                    x = Random.Range(bounds.xMin, bounds.xMax),
                    y = Random.Range(bounds.yMin, bounds.yMax),
                    z = 0
                };
            } while (RoomData.tilemap.HasTile(randomPos) && ++failSafe <= 200);

            result = failSafe <= 200;

            return randomPos;
        }

        private void SpawnWingedEnemyLocation(FlyingEnemyController enemy,
            out bool result)
        {
            var cell = GetRandomEmptyCell(out result);
            if (!result)
            {
                return;
            }

            if (Enumerable.Range(0, 3)
                .Select(i => new Vector3Int(cell.x, cell.y + i, 0))
                .Any(c => RoomData.tilemap.GetTile(c) != null))
            {
                result = false;
                return;
            }

            var position = RoomData.tilemap.GetCellCenterWorld(cell);

            if (_spawnedEnemies.Any(controller => Vector2.Distance(controller
                .transform.position, position) < 3f))
            {
                result = false;
                return;
            }

            InstantiateEnemy(enemy, position, out var instance);
            var startState = Enumerable.Range(0, 2)
                .Select(i => new Vector3Int(cell.x, cell.y - i, 0))
                .Any(c => RoomData.tilemap.HasTile(c))
                ? FlyingEnemyController.StartState.Grounded
                : FlyingEnemyController.StartState.Flying;

            // TODO: singleton usage is ugly here...
            instance.Initialize(FrequentlyAccessed.Instance.playerObject.transform,
                startState);
        }

        private void InstantiateEnemy<T>(T enemy, Vector2 position,
            out T instance) where T : EnemyController
        {
            if (!_enemyChild)
            {
                _enemyChild = new GameObject("Enemies");
                _enemyChild.transform.SetParent(transform);
            }

            instance = Instantiate(enemy, position, Quaternion.identity);
            instance.transform.SetParent(_enemyChild.transform);
            _spawnedEnemies.Add(instance);
        }

        protected void SpawnEnemy(EnemyController enemy, out bool result)
        {
            result = false;

            if (enemy is FlyingEnemyController enemyController)
            {
                SpawnWingedEnemyLocation(enemyController, out result);
            }
        }
    }
}