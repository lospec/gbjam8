using System.Collections.Generic;
using RoomLoading;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Editor.CustomEditor
{
    [UnityEditor.CustomEditor(typeof(RoomData))]
    public class RoomDataEditor : UnityEditor.Editor
    {
        private const string ResourcePath = "Assets/Editor/Resource/RoomResource.asset";

        public int wallWidth = 5;
        public int spaceBetweenWall = 18;
        public int roomHeight = 36;
        public RoomGeneratorResource roomResource;

        private bool foldout = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foldout = EditorGUILayout.Foldout(foldout, "Generator", true);

            if (!foldout)
            {
                return;
            }

            GUILayout.Label("Generator Properties", EditorStyles.boldLabel);
            wallWidth = EditorGUILayout.IntField("Wall Width", wallWidth);
            spaceBetweenWall =
                EditorGUILayout.IntField("Space Between Walls",
                    (spaceBetweenWall / 2) * 2);

            roomHeight = EditorGUILayout.IntField("Room Height", roomHeight);


            if (!roomResource)
            {
                EditorGUILayout.HelpBox("Load Room Resource First!", MessageType
                    .Warning, true);
                if (GUILayout.Button("Load Room Resource"))
                {
                    var height = ((RoomData) target).GetRoomHeight();
                    roomHeight = height;
                    
                    if (!roomResource)
                    {
                        var asset =
                            AssetDatabase.LoadAssetAtPath<RoomGeneratorResource>(
                                ResourcePath);
                        if (asset)
                        {
                            roomResource = asset;
                        }
                        else
                        {
                            roomResource = CreateInstance<RoomGeneratorResource>();
                            AssetDatabase.CreateAsset(roomResource, ResourcePath);
                        }
                    }
                }

                return;
            }

            roomResource.ruleTile = (TileBase) EditorGUILayout.ObjectField(
                "Rule Tile",
                roomResource.ruleTile, typeof(TileBase), false);
            roomResource.leftTile = (TileBase) EditorGUILayout.ObjectField(
                "Left Tile",
                roomResource.leftTile, typeof(TileBase), false);
            roomResource.centerTile =
                (TileBase) EditorGUILayout.ObjectField("Center Tile",
                    roomResource.centerTile, typeof(TileBase), false);
            roomResource.rightTile = (TileBase) EditorGUILayout.ObjectField(
                "Right Tile",
                roomResource.rightTile, typeof(TileBase), false);


            if (GUILayout.Button("Refresh Room Data"))
            {
                var height = ((RoomData) target).GetRoomHeight();
                roomHeight = height;
            }

            if (GUILayout.Button("Generate"))
            {
                GenerateSideWall();
            }

            if (GUILayout.Button("Clear Walls"))
            {
                ClearWalls();
            }

            if (GUILayout.Button("Clear Room"))
            {
                ClearRoom();
            }

            if (GUILayout.Button("Clear All"))
            {
                ClearAll();
            }
        }

        private void ClearRoom()
        {
            var room = (RoomData) target;
            var tilemap = room.tilemap;
            var walls = new List<Vector3Int>();
            for (var y = 0; y < roomHeight; y++)
            {
                for (var x = 0; x < wallWidth; x++)
                {
                    var xi = -x - 1 - spaceBetweenWall / 2;
                    var xj = x + spaceBetweenWall / 2;
                    walls.Add(new Vector3Int(xi, y, 0));
                    walls.Add(new Vector3Int(xj, y, 0));
                }
            }

            foreach (var position in tilemap.cellBounds.allPositionsWithin)
            {
                if (!walls.Contains(position))
                {
                    tilemap.SetTile(position, null);
                }
            }
        }


        private void GenerateSideWall()
        {
            if (EditorUtility.IsDirty(roomResource))
            {
                AssetDatabase.SaveAssets();
            }

            var room = (RoomData) target;
            var tilemap = room.tilemap;
            var borderBlender = room.borderBlender;
            for (var y = 0; y < roomHeight; y++)
            {
                for (var x = 0; x < wallWidth; x++)
                {
                    var xi = -x - 1 - spaceBetweenWall / 2;
                    var xj = x + spaceBetweenWall / 2;
                    tilemap.SetTile(new Vector3Int(xi, y, 0), roomResource.ruleTile);
                    tilemap.SetTile(new Vector3Int(xj, y, 0), roomResource.ruleTile);
                }
            }

            for (var x = 0; x < wallWidth; x++)
            {
                var xi = x - spaceBetweenWall / 2 - wallWidth;
                var xj = x + spaceBetweenWall / 2;
                var tile = roomResource.centerTile;

                if (x == 0)
                {
                    tile = roomResource.leftTile;
                }
                else if (x == wallWidth - 1)
                {
                    tile = roomResource.rightTile;
                }

                for (var i = 0; i < 2; i++)
                {
                    borderBlender.SetTile(new Vector3Int(xi, i * (roomHeight - 1), 0),
                        tile);
                    borderBlender.SetTile(new Vector3Int(xj, i * (roomHeight - 1), 0),
                        tile);
                }
            }
        }

        private void ClearWalls()
        {
            var room = (RoomData) target;
            var tilemap = room.tilemap;
            var borderBlender = room.borderBlender;
            for (var y = 0; y < roomHeight; y++)
            {
                for (var x = 0; x < wallWidth; x++)
                {
                    var xi = -x - 1 - spaceBetweenWall / 2;
                    var xj = x + spaceBetweenWall / 2;
                    tilemap.SetTile(new Vector3Int(xi, y, 0), null);
                    tilemap.SetTile(new Vector3Int(xj, y, 0), null);
                }
            }

            for (var x = 0; x < wallWidth; x++)
            {
                var xi = x - spaceBetweenWall / 2 - wallWidth;
                var xj = x + spaceBetweenWall / 2;

                for (var i = 0; i < 2; i++)
                {
                    borderBlender.SetTile(new Vector3Int(xi, i * (roomHeight - 1), 0),
                        null);
                    borderBlender.SetTile(new Vector3Int(xj, i * (roomHeight - 1), 0),
                        null);
                }
            }
        }

        private void ClearAll()
        {
            var room = (RoomData) target;
            var tilemap = room.tilemap;
            var borderBlender = room.borderBlender;
            tilemap.ClearAllTiles();
            borderBlender.ClearAllTiles();
        }
    }
}