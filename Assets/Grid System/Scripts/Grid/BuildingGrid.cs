using System;
using System.Collections.Generic;
using System.Linq;
using TGL.GridSystem.config;
using TGL.GridSystem.placements;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TGL.GridSystem.Grid
{
    /// <summary>
    /// Used to build a grid for building placement
    /// </summary>
    public class BuildingGrid : MonoBehaviour
    {
        /// <summary>
        /// no of cells in the width
        /// </summary>
        [Tooltip("no of cells in the width"), SerializeField] private int width;
        /// <summary>
        /// no of cells in the height
        /// </summary>
        [Tooltip("no of cells in the height"), SerializeField] private int height;
        /// <summary>
        /// The Grid Cells
        /// </summary>
        private BuildingGridCell[,] grid;
        
        #if UNITY_EDITOR
        [Space(15), Header("Gizmos Settings")]
        public Color gizmosColor = Color.yellow;
        public float gizmoWidth = 0.5f;
        public float gizmoY = 0.01f;
        #endif
        
        private void Start()
        {
            grid = new BuildingGridCell[width, height];
            for (int x = 0; x < grid.GetLength(0); x++) // width
            {
                for (int y = 0; y < grid.GetLength(1); y++) // height
                {
                    grid[x, y] = new BuildingGridCell();
                }
            }
        }
        
        /// <summary>
        /// Uses FloorToInt to convert world position to grid position
        /// FloorToInt is used to ensure that any position within a cell maps to that cell's grid coordinates without exceeding bounds of the building due to non-uniform shapes.
        /// </summary>
        /// <param name="worldPosition"></param>
        /// <returns></returns>
        public Vector2Int WorldToGridPosition(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt((worldPosition - transform.position).x / GridBuildingConfig.cellSize);
            int z = Mathf.FloorToInt((worldPosition - transform.position).z / GridBuildingConfig.cellSize);
            return new Vector2Int(x, z);
        }

        public Vector3 GetCellLowerEdgePos(Vector3 worldPosition)
        {
            Vector2Int cellNum = WorldToGridPosition(worldPosition);
            Vector3 lowerEdgeOfCell = new Vector3(cellNum.x * GridBuildingConfig.cellSize, 0, cellNum.y * GridBuildingConfig.cellSize);
            return  lowerEdgeOfCell;
        }
        
        public bool CanBuild(List<Vector3> allBuildingPositions, bool showLogs)
        {
            foreach (var position in allBuildingPositions)
            {
                Vector2Int gridPosition = WorldToGridPosition(position);
                // Check if the position is within bounds
                if (gridPosition.x < 0 || gridPosition.x >= width || gridPosition.y < 0 || gridPosition.y >= height)
                {
                    return false; // Out of grid bounds
                }
                
                // Check if the cell is already occupied
                if (!grid[gridPosition.x, gridPosition.y].IsEmpty())
                {
                    return false; // Cell is occupied
                }
            }

            if (showLogs)
            {
                Debug.Log($"Checked positions are valid and unoccupied." + string.Join(", ", allBuildingPositions.Select(pos => $"({pos.x:00}, {pos.y:00}, {pos.z:00})")));
                Debug.Log($"The Grid Positions are: " + string.Join(", ", allBuildingPositions.Select(pos => $"({WorldToGridPosition(pos).x}, {WorldToGridPosition(pos).y})")));
            }
            
            return true; // All positions are valid and unoccupied
        }
        
        /// <summary>
        /// Saves an instantiated building into the grid cells
        /// </summary>
        /// <param name="building">The instantiated building</param>
        /// <param name="allBuildingPositions">the position this building occupies</param>
        public void SetBuilding(Building building, List<Vector3> allBuildingPositions)
        {
            foreach (var position in allBuildingPositions)
            {
                Vector2Int gridPosition = WorldToGridPosition(position);
                grid[gridPosition.x, gridPosition.y].SetBuilding(building);
            }
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = gizmosColor;
            if (GridBuildingConfig.cellSize <= 0 || width <= 0 || height <= 0) return;
            Vector3 origin = transform.position;
        
            // Draw horizontal lines (rows)
            for (int y = 0; y <= height; y++)
            {
                Vector3 startPos = origin + new Vector3(0, gizmoY, y * GridBuildingConfig.cellSize);
                Vector3 endPos = origin + new Vector3(width * GridBuildingConfig.cellSize, gizmoY, y * GridBuildingConfig.cellSize);
                Vector3 center = (startPos + endPos) / 2f;
                float length = Vector3.Distance(startPos, endPos);
                Vector3 size = new Vector3(length, gizmoY, gizmoWidth);
                Quaternion rotation = Quaternion.identity;
                Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
                Gizmos.DrawCube(Vector3.zero, size);
                Gizmos.matrix = Matrix4x4.identity;
            }
        
            // Draw vertical lines (columns)
            for (int x = 0; x <= width; x++)
            {
                Vector3 startPos = origin + new Vector3(x * GridBuildingConfig.cellSize, gizmoY, 0);
                Vector3 endPos = origin + new Vector3(x * GridBuildingConfig.cellSize, gizmoY, height * GridBuildingConfig.cellSize);
                Vector3 center = (startPos + endPos) / 2f;
                float length = Vector3.Distance(startPos, endPos);
                Vector3 size = new Vector3(gizmoWidth, gizmoY, length);
                Quaternion rotation = Quaternion.identity;
                Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
                Gizmos.DrawCube(Vector3.zero, size);
                Gizmos.matrix = Matrix4x4.identity;
            }
        
        #if UNITY_EDITOR
            // Draw cell indices
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 cellCenter = origin + new Vector3(
                        (x + 0.5f) * GridBuildingConfig.cellSize,
                        gizmoY,
                        (y + 0.5f) * GridBuildingConfig.cellSize
                    );
                    Handles.Label(cellCenter, $"({x},{y})");
                }
            }
        #endif
        }
        

    }
}
