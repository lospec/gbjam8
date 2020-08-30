using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLoader : MonoBehaviour
{
    [Header("Load / Unload distances")]
    // Distance from the top room
    public float loadDistance;
    public float unloadDistance;

    [Header("Rooms")]
    // Path from which rooms are loaded
    public string roomPath;
    // Start room
    public GameObject startRoom;

    [Header("Other")]
    // Level difficulty
    public int levelDifficulty;
    // Start offset from ground room
    public float startOffset;

    private GameObject player;
    private float currentMaxY;

    private Object[] rooms;
    private GameObject topRoom;
    private List<GameObject> loadedRooms;

    // Start is called before the first frame update
    void Start()
    {
        loadedRooms = new List<GameObject>();

        rooms = Resources.LoadAll(roomPath);
        player = FrequentlyAccessed.Instance.playerObject;

        // Instantiating the first room
        topRoom = Instantiate(startRoom, Vector3.zero, Quaternion.Euler(Vector3.zero));
        currentMaxY = topRoom.GetComponent<RoomData>().GetMidPoint() + startOffset;

        Debug.Log("Max y: " + currentMaxY);
        Debug.Log("top: " + currentMaxY + topRoom.GetComponent<RoomData>().GetRoomHeight() / 2f);
    }

    // Update is called once per frame
    void Update()
    {
        // Getting the distance from the player to the top room
        float distance = Vector2.Distance(topRoom.transform.position, player.transform.position);

        if (distance < loadDistance)
        {
            LoadRoom();
        }

        UnloadRooms();
    }

    private void LoadRoom()
    {
        // Name of the chosenroom
        string chosenRoomName = "TemplateStart";
        // Room to instantiate
        GameObject chosenRoom = null;
        // Room data of the room
        RoomData roomData;
        // Y coord of the new room
        float yPos;

        // Avoid choosing the template room or a start room
        while (chosenRoomName.Contains("Template") || chosenRoomName.Contains("Start"))
        {
            chosenRoom = (GameObject)rooms[Random.Range(0, rooms.Length)];
            chosenRoomName = chosenRoom.name;
        }

        roomData = chosenRoom.GetComponent<RoomData>();

        if (chosenRoom != null)
        {
            yPos = currentMaxY + roomData.GetRoomHeight() / 2f + topRoom.GetComponent<RoomData>().GetRoomHeight() / 2f +
                ((roomData.GetRoomHeight() % 2 == 0) ? -1 : 0);
            // Instantiating the room
            topRoom = Instantiate(chosenRoom, new Vector3(topRoom.transform.position.x, yPos), Quaternion.Euler(Vector3.zero));

            loadedRooms.Add(topRoom);
            currentMaxY = yPos;
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
        List<GameObject> roomCopy = new List<GameObject>(loadedRooms);

        for (int i=0; i< roomCopy.Count; i++)
        {
            // If I'm far enough from the room and the player is above it (I can't destroy rooms above the player)
            if (Vector2.Distance(roomCopy[i].transform.position, player.transform.position) > unloadDistance &&
                player.transform.position.y > roomCopy[i].transform.position.y)
            {
                // I remove it from the list of the loaded rooms
                loadedRooms.Remove(roomCopy[i]);
                // And I destroy it
                Destroy(roomCopy[i]);
            }
        }
    }
}
