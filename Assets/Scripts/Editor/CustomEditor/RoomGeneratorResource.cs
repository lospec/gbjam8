using UnityEngine;
using UnityEngine.Tilemaps;

namespace Editor.CustomEditor
{
    public class RoomGeneratorResource : ScriptableObject
    {
        public TileBase ruleTile = null;
        public TileBase leftTile = null;
        public TileBase centerTile = null;
        public TileBase rightTile = null;
    }
}