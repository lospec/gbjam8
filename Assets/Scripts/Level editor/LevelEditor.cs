using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Inputs;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using Weapon.Hook;

/**
 * TODO: 
 * - Possibilità di nascondere nemici e asset per poter disegnare meglio
 */

[System.Serializable]
public class SavedItem
{
    public float x;
    public float y;
    public string name;
}

[System.Serializable]
public class SaveUtility
{
    public SavedItem[] items;
}

public class LevelEditor : MonoBehaviour, PlayerControls.ICameraActions
{
    public enum Tool {Pencil, Eraser, Fill, Select, Move}
    public static LevelEditor Instance;
    public Tilemap tilemap;
    public RuleTile defaultTile;
    public GameObject brushPreview;
    public GameObject selectedIcon;
    public EventSystem uiEvents;
    public Text fileName;
    public float cameraSpeed = 5f;
    [Header("Please mark as readonly I don't know how to do that")]
    public bool isDragging;
    public bool cursorOnUI;
    public bool isDrawing;
    public bool canDraw;

    // Input manager
    private PlayerControls _input = default;
    // Using private members so that update isn't too expensive
    private Vector3 mousePosition;
    private Vector3 mouseWorldPosition;
    private Vector3 mouseTilemapPosition;
    private Vector3Int intTilemapPosition;
    private Vector3 playerStartPos;

    private Tool currentTool;
    private UnityEngine.Camera camera;
    private Vector2 cameraVelocity;

    private GameObject prevBrushPreview;
    private GameObject selectedAsset;

    private List<GameObject> instantiatedAssets;
    private List<Vector2> startPositions;
    private List<(float, float, string)> tilemapData;

    private void Awake()
    {
        _input = new PlayerControls();

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
        playerStartPos = FrequentlyAccessed.Instance.playerObject.transform.position;

        instantiatedAssets = new List<GameObject>();
        startPositions = new List<Vector2>();
        tilemapData = new List<(float, float, string)>();

        _input.Camera.CameraMovement.performed += OnCameraMovement;
        _input.Camera.CameraMovement.canceled += _ => cameraVelocity = Vector2.zero;
        _input.Camera.Delete.started += OnDelete;

        _input.Camera.Enable();

        canDraw = true;
        // Default tool is pencil
        currentTool = Tool.Pencil;
        // Freezing time so that the user can create the level
        Time.timeScale = 0;
        selectedIcon.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        bool leftMousePressed = Mouse.current.leftButton.isPressed;
        bool rightMousePressed = Mouse.current.rightButton.isPressed;
        Collider2D overlapped;

        mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = camera.nearClipPlane;

        mouseWorldPosition = camera.ScreenToWorldPoint(mousePosition);
        mouseTilemapPosition = tilemap.WorldToCell(mouseWorldPosition);

        intTilemapPosition.x = Mathf.RoundToInt(mouseTilemapPosition.x);
        intTilemapPosition.y = Mathf.RoundToInt(mouseTilemapPosition.y);

        brushPreview.transform.position = mouseWorldPosition;
        overlapped = Physics2D.OverlapPoint(mouseWorldPosition);

        if (overlapped != null)
        {
            if (overlapped.transform.root.GetComponentInChildren<MovableAsset>() != null)
            {
                canDraw = false;
                isDrawing = false;
            }
        }
        else
        {
            canDraw = true;
        }

        if (!uiEvents.IsPointerOverGameObject())
        {
            cursorOnUI = false;

            if (!isDragging && canDraw)
            {
                switch (currentTool)
                {
                    case Tool.Pencil:
                        if (leftMousePressed)
                        {
                            isDrawing = true;
                            // Saving data
                            AddTileToList(intTilemapPosition.x, intTilemapPosition.y, "tile");

                            // Paint the current tile
                            tilemap.SetTile(intTilemapPosition, defaultTile);
                            tilemap.RefreshAllTiles();
                        }
                        else if (rightMousePressed)
                        {
                            isDrawing = true;
                            // Saving data
                            AddTileToList(intTilemapPosition.x, intTilemapPosition.y, "erase");

                            // Clear the current tile
                            tilemap.SetTile(intTilemapPosition, null);
                        }
                        else
                        {
                            isDrawing = false;
                        }

                        break;
                    case Tool.Eraser:
                        if (leftMousePressed)
                        {
                            isDrawing = true;
                            // Saving data
                            AddTileToList(intTilemapPosition.x, intTilemapPosition.y, "erase");

                            tilemap.SetTile(intTilemapPosition, null);
                        }
                        else
                        {
                            isDrawing = false;
                        }

                        break;
                    case Tool.Fill:
                        if (Mouse.current.leftButton.wasReleasedThisFrame)
                        {
                            isDrawing = true;
                            // Saving data
                            AddTileToList(intTilemapPosition.x, intTilemapPosition.y, "fill");

                            tilemap.FloodFill(intTilemapPosition, defaultTile);
                        }
                        else
                        {
                            isDrawing = false;
                        }

                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            cursorOnUI = true;
        }

        MoveCamera();
    }

    private void AddTileToList(float x, float y, string name)
    {
        if (tilemapData.Contains((x, y, name)))
        {
            tilemapData.Remove((x, y, name));
        }

        tilemapData.Add((x, y, name));
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

    public void AddToAssetList(GameObject toAdd)
    {
        instantiatedAssets.Add(toAdd);
    }

    public void RemoveFromAssetList(GameObject toRemove)
    {
        instantiatedAssets.Remove(toRemove);
    }
    
    public void StartPlayMode()
    {
        Time.timeScale = 1;

        for (int i=0; i<instantiatedAssets.Count; i++)
        {
            MonoBehaviour[] behaviours = instantiatedAssets[i].GetComponentsInChildren<MonoBehaviour>();
            startPositions.Add(instantiatedAssets[i].transform.position);

            for (int j=0; j<behaviours.Length; j++)
            {
                behaviours[j].enabled = true;
            }

            Destroy(instantiatedAssets[i].GetComponent<PolygonCollider2D>());
        }
    }

    public void EndPlayMode()
    {
        Time.timeScale = 0;

        FrequentlyAccessed.Instance.playerObject.GetComponentInChildren<GrapplingGun>().transform.GetChild(0).GetComponent<LineRenderer>().enabled = false;

        for (int i = 0; i < instantiatedAssets.Count; i++)
        {
            MonoBehaviour[] behaviours = instantiatedAssets[i].GetComponentsInChildren<MonoBehaviour>();
            instantiatedAssets[i].transform.position = startPositions[i];

            for (int j = 0; j < behaviours.Length; j++)
            {
                behaviours[j].enabled = false;
            }

            instantiatedAssets[i].AddComponent<PolygonCollider2D>();
        }

        FrequentlyAccessed.Instance.playerObject.transform.position = playerStartPos;
    }

    public void SetSelected(GameObject toSet)
    {
        if (selectedAsset != null)
        {
            selectedAsset.GetComponent<MovableAsset>().Deselect();
        }

        selectedAsset = toSet;
    }

    public void ResetSelected(GameObject toReset)
    {
        if (toReset.Equals(selectedAsset))
        {
            selectedAsset = null;
        }
    }

    public void Save()
    {
        List<(float, float, string)> toSave = new List<(float, float, string)>();
        SaveUtility saveUtility = new SaveUtility();
        string dataJSON;
        int j = 0;

        SaveSystem.Instance.Initialize(fileName.text + ".bin");

        // Saving all the instantiated assets to the tilemap data
        for (int i=0; i<instantiatedAssets.Count; i++)
        {
            Vector2 currentPos = instantiatedAssets[i].transform.position;

            AddTileToList(currentPos.x, currentPos.y, instantiatedAssets[i].name);
        }

        saveUtility.items = new SavedItem[tilemapData.Count];

        // Saving the tilemap data to the list
        foreach ((float, float, string) value in tilemapData)
        {
            // Creating a new saved item
            saveUtility.items[j] = new SavedItem();
            saveUtility.items[j].x = value.Item1;
            saveUtility.items[j].y = value.Item2;
            saveUtility.items[j].name = value.Item3;

            j++;
        }
        dataJSON = JsonUtility.ToJson(saveUtility);
  
        SaveSystem.Instance.SetString("TileMap", dataJSON);
        Debug.Log(dataJSON);
        SaveSystem.Instance.SaveToDisk();
    }

    public void OnDelete(InputAction.CallbackContext context)
    {
        if (selectedAsset != null)
        {
            selectedIcon.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
            RemoveFromAssetList(selectedAsset);
            Destroy(selectedAsset);
        }
    }

    public void Load()
    {
        string json;
        SaveUtility saveUtility;
        SaveSystem.Instance.Initialize(fileName.text);
        SaveSystem.Instance.Load();

        Vector3Int tilePosition = new Vector3Int();
        Vector3 assetPosition = new Vector3();

        Debug.Log("Carico");
        
        if (SaveSystem.Instance.HasKey("TileMap"))
        {
            json = SaveSystem.Instance.GetString("TileMap");
            saveUtility = JsonUtility.FromJson<SaveUtility>(json);

            for (int i=0; i<saveUtility.items.Length; i++)
            {
                SavedItem currentItem = saveUtility.items[i];

                tilePosition.x = Mathf.RoundToInt(currentItem.x);
                tilePosition.y = Mathf.RoundToInt(currentItem.y);

                assetPosition.x = currentItem.x;
                assetPosition.y = currentItem.y;

                switch (currentItem.name)
                {
                    case "tile":
                        tilemap.SetTile(tilePosition, defaultTile);
                        break;
                    case "erase":
                        tilemap.SetTile(tilePosition, null);
                        break;
                    case "fill":
                    case "flood":
                        Debug.LogError("sto per flooddare");
                        // ISSUE: make sure floods are made at the end 
                        tilemap.FloodFill(tilePosition, defaultTile);
                        break;
                    default:
                        Debug.Log(currentItem.name);

                        // load prefab
                        GameObject prefab = Instantiate((GameObject)Resources.Load(currentItem.name), 
                            assetPosition, Quaternion.Euler(Vector3.zero));
                        prefab.transform.parent = tilemap.transform;

                        break;
                }
            }
        }
    }
}
