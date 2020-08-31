using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using Utility;

namespace RoomLoading
{
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

        private bool _readHeight = false;
        
        public int GetRoomHeight()
        {
            if (_readHeight)
            {
                return height;
            }

            _readHeight = true;

            // Cellbounds might not be up to date until CompressBounds is called
            tilemap.CompressBounds();
            borderBlender.CompressBounds();

            var tilemapBound = tilemap.cellBounds;
            var borderBounds = borderBlender.cellBounds;
            minY = borderBounds.yMin <= tilemapBound.yMin
                ? borderBounds.yMin
                : tilemapBound.yMin;
            maxY = borderBounds.yMax >= tilemapBound.yMax
                ? borderBounds.yMax
                : tilemapBound.yMax;
            height = maxY - minY;
            return height;
        }

        // not used in spawning anymore, will keep in case it comes in handy
        public float GetMidPoint()
        {
            GetRoomHeight();

            return borderBlender
                .CellToWorld(new Vector3Int(0, (maxY + minY + 1) / 2,
                    tilemap.cellBounds.z))
                .y;
        }
    }
}