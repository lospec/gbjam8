using System;
using System.Collections.Generic;
using System.Linq;
using Enemy;
using RoomLoading;
using UnityEngine;
using Utility;
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

        private void SpawnFlyingEnemyLocation(FlyingEnemyController enemy,
            Vector3Int cell, out bool result)
        {
            if (Enumerable.Range(0, 3)
                .Select(i => new Vector3Int(cell.x, cell.y + i, 0))
                .Any(c => RoomData.tilemap.GetTile(c) != null))
            {
                result = false;
                return;
            }

            var position = RoomData.tilemap.GetCellCenterWorld(cell);

            if (AnyNearbySpawned(out result, 3f, position)) return;


            InstantiateEnemy(enemy, position, out var instance);
            var startState = Enumerable.Range(0, 2)
                .Select(i => new Vector3Int(cell.x, cell.y - i, 0))
                .Any(c => RoomData.tilemap.HasTile(c))
                ? FlyingEnemyController.StartState.Grounded
                : FlyingEnemyController.StartState.Flying;

            // TODO: singleton usage is ugly here...
            instance.Initialize(GameManager.instance.playerObject.transform,
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

            var cell = GetRandomEmptyCell(out result);
            if (!result)
            {
                return;
            }

            switch (enemy)
            {
                case FlyingEnemyController flyingEnemyController:
                    SpawnFlyingEnemyLocation(flyingEnemyController, cell,
                        out result);
                    break;
                case HorizontalMoveEnemyController horizontalMoveEnemyController:
                    SpawnHorizontalMoveEnemyLocation(horizontalMoveEnemyController,
                        cell, out result);
                    break;
                case RandomMoveEnemyController randomMoveEnemyController:
                    SpawnRandomMoveEnemyLocation(randomMoveEnemyController, cell,
                        out result);
                    break;
            }
        }

        private void SpawnRandomMoveEnemyLocation(RandomMoveEnemyController enemy,
            Vector3Int cell,
            out bool result)
        {
            var range = Enumerable.Range(0, 2).ToArray();
            if (range.Zip(range, (i, j) => new Vector3Int(i, j, 0)).Any(c
                => RoomData.tilemap.HasTile(c)))
            {
                result = false;
                return;
            }

            var position = RoomData.tilemap.GetCellCenterWorld(cell);

            if (AnyNearbySpawned(out result, 3f, position)) return;

            InstantiateEnemy(enemy, position, out _);
        }

        private bool AnyNearbySpawned(out bool result, float distance,
            Vector3 position)
        {
            result = true;
            
            if (_spawnedEnemies.Any(controller => Vector2.Distance(controller
                .transform.position, position) < distance))
            {
                result = false;
                return true;
            }

            return false;
        }

        private void SpawnHorizontalMoveEnemyLocation(
            HorizontalMoveEnemyController enemy, Vector3Int cell,
            out bool result)
        {
            if (Enumerable.Range(-2, 5).Select(i => new Vector3Int(cell.x + i, cell
                .y, 0)).Any(c => _roomData.tilemap.HasTile(c)))
            {
                result = false;
                return;
            }

            var position = RoomData.tilemap.GetCellCenterWorld(cell);

            if (AnyNearbySpawned(out result, 3f, position)) return;

            InstantiateEnemy(enemy, position, out _);
        }
    }
}