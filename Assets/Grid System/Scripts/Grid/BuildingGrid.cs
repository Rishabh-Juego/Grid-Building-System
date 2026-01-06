using System;
using System.Collections.Generic;
using TGL.GridSystem.Buildings.UnSure;
using TGL.GridSystem.config;
using UnityEngine;

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
        
        public Vector2Int WorldToGridPosition(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt((worldPosition - transform.position).x / GridBuildingConfig.cellSize);
            int z = Mathf.FloorToInt((worldPosition - transform.position).z / GridBuildingConfig.cellSize);
            return new Vector2Int(x, z);
        }
        
        public bool CanBuild(List<Vector3> allBuildingPositions)
        {
            foreach (var position in allBuildingPositions)
            {
                Vector2Int gridPosition = WorldToGridPosition(position);
                // Check if the position is within bounds
                if (gridPosition.x < 0 || gridPosition.x >= width || gridPosition.y < 0 || gridPosition.y >= height)
                {
                    return false; // Out of bounds
                }
                // Check if the cell is already occupied
                if (!grid[gridPosition.x, gridPosition.y].IsEmpty())
                {
                    return false; // Cell is occupied
                }
            }
            return true; // All positions are valid and unoccupied
        }
        
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
            Gizmos.color = Color.yellow;
            if(GridBuildingConfig.cellSize <= 0 || width <= 0 || height <= 0) return;
            Vector3 origin = transform.position;
            for(int y = 0; y <= height; y++)
            {
                Vector3 startPos = origin + new Vector3(0, 0.01f, y * GridBuildingConfig.cellSize);
                Vector3 endPos = origin + new Vector3(width * GridBuildingConfig.cellSize, 0.01f, y * GridBuildingConfig.cellSize);
                Gizmos.DrawLine(startPos, endPos);
            }

            for (int x = 0; x <= width; x++)
            {
                Vector3 startPos = origin + new Vector3(x * GridBuildingConfig.cellSize, 0.01f, 0);
                Vector3 endPos = origin + new Vector3(x * GridBuildingConfig.cellSize, 0.01f, height * GridBuildingConfig.cellSize);
                Gizmos.DrawLine(startPos, endPos);
            }
        }
    }
}
