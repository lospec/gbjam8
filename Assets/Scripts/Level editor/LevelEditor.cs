using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Inputs;

public class LevelEditor : MonoBehaviour, PlayerControls.ICameraActions
{
    public enum Tool {Pencil, Eraser, Fill, Select, Move}
    public static LevelEditor Instance;
    public Tilemap tilemap;
    public GameObject brushPreview;
    public float cameraSpeed = 5f;

    // Input manager
    private PlayerControls _input = default;
    // Using private members so that update isn't too expensive
    private Vector3 mousePosition;
    private Vector3 mouseWorldPosition;
    private Vector3 mouseTilemapPosition;

    private Tool currentTool;
    private Camera camera;
    private Vector2 cameraVelocity;

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
        mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = camera.nearClipPlane;

        mouseWorldPosition = camera.ScreenToWorldPoint(mousePosition);
        mouseTilemapPosition = tilemap.WorldToCell(mouseWorldPosition);

        brushPreview.transform.position = mouseWorldPosition;

        switch (currentTool)
        {
            case Tool.Pencil:
                break;
            case Tool.Eraser:
                break;
            case Tool.Fill:
                break;
            case Tool.Select:
                break;
            case Tool.Move:
                break;
            default:
                break;
        }

        MoveCamera();
    }

    private void MoveCamera()
    {
        Debug.Log(cameraVelocity);
        camera.transform.position += (Vector3)cameraVelocity * cameraSpeed * Time.unscaledDeltaTime;
    }

    public void OnCameraMovement(InputAction.CallbackContext context)
    {
        cameraVelocity = (Vector3)context.ReadValue<Vector2>();
    }
}
