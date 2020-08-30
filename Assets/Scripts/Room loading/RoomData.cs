using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomData : MonoBehaviour
{
    // Room difficulty
    [Range(0, 10)]
    public int difficulty;
    // Tilemap
    public Tilemap tilemap;
    [Header("Public variables just for debug purpose")]
    // Min y (pivot used to connect rooms)
    public int minY;
    public int height;

    public int GetRoomHeight()
    {
        Vector3Int currentTilePos = new Vector3Int();

        int minY = tilemap.size.y;
        int maxY = -tilemap.size.y;

        for (int i = tilemap.cellBounds.xMin; i < tilemap.cellBounds.xMax; i++)
        {
            for (int j = tilemap.cellBounds.yMin; j < tilemap.cellBounds.yMax; j++)
            {
                currentTilePos.x = i;
                currentTilePos.y = j;

                if (tilemap.HasTile(currentTilePos))
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

        return maxY - minY;
    }
}
