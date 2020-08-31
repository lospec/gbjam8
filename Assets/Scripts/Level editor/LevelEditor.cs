using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class LevelEditor : MonoBehaviour
{
    public enum Tools {Pencil, Eraser, Fill, Select, Move}
    public static LevelEditor Instance;
    public Tilemap tilemap;
    public GameObject testObject;

    // Using private members so that update isn't too expensive
    private Vector3 mousePosition;
    private Vector3 mouseWorldPosition;
    private Vector3 mouseTilemapPosition;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        mousePosition = new Vector3();
        mouseWorldPosition = new Vector3();
        mouseTilemapPosition = new Vector3();

        // Freezing time so that the user can create the level
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = FrequentlyAccessed.Instance.camera.nearClipPlane;

        mouseWorldPosition = FrequentlyAccessed.Instance.camera.ScreenToWorldPoint(mousePosition);
        mouseTilemapPosition = tilemap.WorldToCell(mouseWorldPosition / 8);

        testObject.transform.position = mouseWorldPosition;
        Debug.Log("Tile mouse: " + mouseWorldPosition);
    }
}
