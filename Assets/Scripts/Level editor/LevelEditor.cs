using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Inputs;
using UnityEngine.EventSystems;

/**
 * TODO: 
 * - Possibilità di nascondere nemici e asset per poter disegnare meglio
 */

public class LevelEditor : MonoBehaviour, PlayerControls.ICameraActions
{
    public enum Tool {Pencil, Eraser, Fill, Select, Move}
    public static LevelEditor Instance;
    public Tilemap tilemap;
    public RuleTile defaultTile;
    public GameObject brushPreview;
    public EventSystem uiEvents;
    public float cameraSpeed = 5f;
    [Header("Please mark as readonly I don't know how to do that")]
    public bool isDragging;
    public bool cursorOnUI;

    // Input manager
    private PlayerControls _input = default;
    // Using private members so that update isn't too expensive
    private Vector3 mousePosition;
    private Vector3 mouseWorldPosition;
    private Vector3 mouseTilemapPosition;
    private Vector3Int intTilemapPosition;

    private Tool currentTool;
    private Camera camera;
    private Vector2 cameraVelocity;
    private GameObject prevBrushPreview;

    private Dictionary<(int, int), string> tilemapData;

    private void Awake()
    {
        _input = new PlayerControls();
        _input.Camera.Enable();

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        camera = FrequentlyAccessed.Instance.camera;

        mousePosition = new Vector3();
        mouseWorldPosition = new Vector3();
        mouseTilemapPosition = new Vector3();
        cameraVelocity = new Vector3();
        intTilemapPosition = new Vector3Int();
        tilemapData = new Dictionary<(int, int), string>();

        _input.Camera.CameraMovement.performed += OnCameraMovement;
        _input.Camera.CameraMovement.canceled += _ => cameraVelocity = Vector2.zero;

        // Default tool is pencil
        currentTool = Tool.Pencil;
        // Freezing time so that the user can create the level
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        bool leftMousePressed = Mouse.current.leftButton.isPressed;
        bool rightMousePressed = Mouse.current.rightButton.isPressed;

        mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = camera.nearClipPlane;

        mouseWorldPosition = camera.ScreenToWorldPoint(mousePosition);
        mouseTilemapPosition = tilemap.WorldToCell(mouseWorldPosition);

        intTilemapPosition.x = Mathf.RoundToInt(mouseTilemapPosition.x);
        intTilemapPosition.y = Mathf.RoundToInt(mouseTilemapPosition.y);

        brushPreview.transform.position = mouseWorldPosition;

        if (!uiEvents.IsPointerOverGameObject() && !isDragging)
        {
            cursorOnUI = false;

            switch (currentTool)
            {
                case Tool.Pencil:
                    if (leftMousePressed)
                    {
                        // Saving data
                        tilemapData[(intTilemapPosition.x, intTilemapPosition.y)] = "tile";

                        // Paint the current tile
                        tilemap.SetTile(intTilemapPosition, defaultTile);
                        tilemap.RefreshAllTiles();
                    }
                    else if (rightMousePressed)
                    {
                        // Saving data
                        tilemapData.Remove((intTilemapPosition.x, intTilemapPosition.y));

                        // Clear the current tile
                        tilemap.SetTile(intTilemapPosition, null);
                    }
                    break;
                case Tool.Eraser:
                    if (leftMousePressed)
                    {
                        // Saving data
                        tilemapData.Remove((intTilemapPosition.x, intTilemapPosition.y));

                        tilemap.SetTile(intTilemapPosition, null);
                    }
                    break;
                case Tool.Fill:
                    if (Mouse.current.leftButton.wasReleasedThisFrame)
                    {
                        // Saving data
                        tilemapData[(intTilemapPosition.x, intTilemapPosition.y)] = "flood";

                        tilemap.FloodFill(intTilemapPosition, defaultTile);
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            cursorOnUI = true;
        }

        MoveCamera();
    }

    private void MoveCamera()
    {
        camera.transform.position += (Vector3)cameraVelocity * cameraSpeed * Time.unscaledDeltaTime;
    }

    public void OnCameraMovement(InputAction.CallbackContext context)
    {
        cameraVelocity = (Vector3)context.ReadValue<Vector2>();
    }

    public void SetTool(Tool tool)
    {
        currentTool = tool;
    }

    public void SetBrushPreview(GameObject toMove)
    {
        prevBrushPreview = brushPreview;
        brushPreview = toMove;
    }

    public void ResetBrushPreview()
    {
        brushPreview = prevBrushPreview;
    }
}
