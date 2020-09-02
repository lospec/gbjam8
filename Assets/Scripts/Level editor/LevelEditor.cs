﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Inputs;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    private Tool currentTool;
    private Camera camera;
    private Vector2 cameraVelocity;

    private GameObject prevBrushPreview;
    private GameObject selectedAsset;

    private List<GameObject> instantiatedAssets;
    private List<Vector2> startPositions;
    private Dictionary<(float, float), string> tilemapData;

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

        instantiatedAssets = new List<GameObject>();
        startPositions = new List<Vector2>();
        tilemapData = new Dictionary<(float, float), string>();

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
        RaycastHit2D hit;

        mousePosition = Mouse.current.position.ReadValue();
        mousePosition.z = camera.nearClipPlane;

        mouseWorldPosition = camera.ScreenToWorldPoint(mousePosition);
        mouseTilemapPosition = tilemap.WorldToCell(mouseWorldPosition);

        intTilemapPosition.x = Mathf.RoundToInt(mouseTilemapPosition.x);
        intTilemapPosition.y = Mathf.RoundToInt(mouseTilemapPosition.y);

        brushPreview.transform.position = mouseWorldPosition;
        hit = Physics2D.Raycast(mouseWorldPosition, new Vector2(1f, 0f), 0.1f);

        if (hit && hit.collider.GetComponent<MovableAsset>() != null)
        {
            canDraw = false;
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
                            tilemapData[(intTilemapPosition.x, intTilemapPosition.y)] = "tile";

                            // Paint the current tile
                            tilemap.SetTile(intTilemapPosition, defaultTile);
                            tilemap.RefreshAllTiles();
                        }
                        else if (rightMousePressed)
                        {
                            isDrawing = true;
                            // Saving data
                            tilemapData.Remove((intTilemapPosition.x, intTilemapPosition.y));

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
                            tilemapData.Remove((intTilemapPosition.x, intTilemapPosition.y));

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
                            tilemapData[(intTilemapPosition.x, intTilemapPosition.y)] = "flood";

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

        Debug.Log("Added at " + toAdd.transform.position);
        tilemapData[(toAdd.transform.position.x, toAdd.transform.position.y)] = toAdd.name;
    }

    public void RemoveFromAssetList(GameObject toRemove)
    {
        Debug.Log("Removed at " + toRemove.transform.position);
        instantiatedAssets.Remove(toRemove);
        tilemapData.Remove((toRemove.transform.position.x, toRemove.transform.position.y));
    }
    
    public void StartPlayMode()
    {
        Time.timeScale = 1;

        for (int i=0; i<instantiatedAssets.Count; i++)
        { 

            MonoBehaviour[] behaviours = instantiatedAssets[i].GetComponentsInChildren<MonoBehaviour>();
            startPositions.Add(instantiatedAssets[i].transform.position);

            /*
            for (int j=0; j<behaviours.Length; j++)
            {
                behaviours[j].enabled = true;
            }
            */
        }
    }

    public void EndPlayMode()
    {
        Time.timeScale = 0;

        for (int i = 0; i < instantiatedAssets.Count; i++)
        {
            MonoBehaviour[] behaviours = instantiatedAssets[i].GetComponentsInChildren<MonoBehaviour>();
            instantiatedAssets[i].transform.position = startPositions[i];

            for (int j = 0; j < behaviours.Length; j++)
            {
                behaviours[j].enabled = false;
            }
        }
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

            tilemapData[(currentPos.x, currentPos.y)] = instantiatedAssets[i].name;
        }

        saveUtility.items = new SavedItem[tilemapData.Keys.Count];

        // Saving the tilemap data to the list
        foreach ((float, float) key in tilemapData.Keys)
        {
            // Creating a new saved item
            saveUtility.items[j] = new SavedItem();
            saveUtility.items[j].x = key.Item1;
            saveUtility.items[j].y = key.Item2;
            saveUtility.items[j].name = tilemapData[key];

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
}
