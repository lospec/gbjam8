using System.Collections.Generic;
using Spawn;
using UnityEngine;
using Utility;

namespace RoomLoading
{
    public class RoomLoader : MonoBehaviour
    {
        [Header("Load / Unload distances")]
        // Distance from the top room
        public float loadDistance;
        public float unloadDistance;

        [Header("Rooms")]
        // Path from which rooms are loaded
        public GameObject[] rooms;
        // Start room
        public GameObject startRoom;

        [Header("Other")]
        // Level difficulty
        public int levelDifficulty;
        private int _currentMaxY = 0;
        private List<GameObject> _loadedRooms;

        private GameObject _player;

        private int _roomCount;


        private GameObject _topRoom;

        [Header("Deco")]
        [SerializeField] private Transform bottomDecoration;


        // Start is called before the first frame update
        private void Start()
        {
            _loadedRooms = new List<GameObject>();
            _player = GameManager.instance.playerObject;
            var start = SpawnRoom(startRoom);
            PlaceBottomDecoration(start);
        }

        private void PlaceBottomDecoration(GameObject start)
        {
            bottomDecoration.position = start.transform.position;
        }

        // Update is called once per frame
        private void Update()
        {
            // Getting the distance from the player to the top room
            var distance =
                Vector2.Distance(_topRoom.transform.position,
                    _player.transform.position);

            if (distance < loadDistance) LoadRoom();

            UnloadRooms();
        }

        // Keep Spawning code in one place
        private GameObject SpawnRoom(GameObject room)
        {
            _topRoom = Instantiate(room, transform);
            var data = _topRoom.GetComponent<RoomData>();
            data.GetRoomHeight();
            _topRoom.transform.position = new Vector3(0, _currentMaxY + -data.minY);
            _currentMaxY += data.GetRoomHeight();
            _topRoom.name = $"Room {_roomCount++} ({room.name})";
            return _topRoom;
        }

        private void LoadRoom()
        {
            // Room to instantiate
            var chosenRoom = rooms[Random.Range(0, rooms.Length)];
            if (chosenRoom != null)
            {
                var room = SpawnRoom(chosenRoom);
                _loadedRooms.Add(room);
                var spawner = room.GetComponent<EnemySpawner>();
                spawner.SpawnEnemies();
            }
            else
            {
                Debug.LogError("Couldn't instantiate room");
            }
        }

        // Unloads all the rooms that are far enough from the player
        // OPTIMIZABLE: just set a bottom room and destroy it, then set the room above it as the bottom room
        private void UnloadRooms()
        {
            // Using a copy to cycle through the rooms
            var roomCopy = new List<GameObject>(_loadedRooms);

            for (var i = 0; i < roomCopy.Count; i++)
                // If I'm far enough from the room and the player is above it (I can't destroy rooms above the player)
                if (Vector2.Distance(roomCopy[i].transform.position,
                        _player.transform.position) > unloadDistance &&
                    _player.transform.position.y > roomCopy[i].transform.position.y)
                {
                    // I remove it from the list of the loaded rooms
                    _loadedRooms.Remove(roomCopy[i]);
                    // And I destroy it
                    Destroy(roomCopy[i]);
                }
        }
    }
}