using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

public class RoomData : MonoBehaviour
{
    // Room difficulty
    [Range(0, 10)]
    public int difficulty;
    // Tilemap
    public Tilemap tilemap;
    public Tilemap borderBlender;
    
    [Header("Public variables just for debug purpose")]
    [InspectorReadOnly] public int minY;
    [InspectorReadOnly] public int maxY;
    [InspectorReadOnly] public int height = -1;

    public int GetRoomHeight()
    {
        Vector3Int currentTilePos = new Vector3Int();

        minY = tilemap.size.y;
        maxY = -tilemap.size.y;

        for (int i = borderBlender.cellBounds.xMin; i < borderBlender.cellBounds.xMax; i++)
        {
            for (int j = borderBlender.cellBounds.yMin; j < borderBlender.cellBounds.yMax; j++)
            {
                currentTilePos.x = i;
                currentTilePos.y = j;

                if (borderBlender.HasTile(currentTilePos))
                {
                    if (j < minY)
                    {
                        minY = j;
                    }

                    if (j > maxY)
                    {
                        maxY = j;
                    }
                }
            }
        }

        height = maxY - minY + 1;

        Debug.Log("room " + name + ", max: " + maxY + ", min: " + minY);

        return height;
    }

    public float GetMidPoint()
    {
        GetRoomHeight();

        return borderBlender.CellToWorld(new Vector3Int(0, (maxY + minY + 1) / 2, tilemap.cellBounds.z)).y;
    }
}
