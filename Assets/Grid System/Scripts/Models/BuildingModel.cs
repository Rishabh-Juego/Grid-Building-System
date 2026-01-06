using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TGL.GridSystem.Buildings
{
    /// <summary>
    /// Has the building model details with shape units for grid occupation
    /// </summary>
    public class BuildingModel : MonoBehaviour
    {
        [SerializeField] private string buildingName;
        // [SerializeField] private Vector2Int size; // Size in grid units (width, height)
        // [SerializeField] private Sprite icon; // Icon for UI representation can be added in the scriptable object
        public string BuildingName => buildingName;

        /// <summary>
        /// The model wrapper to rotate the building model, not the entire game object
        /// </summary>
        [SerializeField] private Transform modelWrapper;
        public float RotationY { get => modelWrapper.localEulerAngles.y; }
        
        /// <summary>
        /// points that this building occupies in the grid
        /// </summary>
        private BuildingShapeUnit[] shapeUnits;

        private void Awake()
        {
            shapeUnits = GetComponentsInChildren<BuildingShapeUnit>();
        }
        
        public void RotateModel(int rotationStep)
        {
            modelWrapper.Rotate(Vector3.up, rotationStep);
            // modelWrapper.Rotate(0, rotationStep, 0);
        }
        
        public List<Vector3> GetAllBuildingShapePositions()
        {
            return shapeUnits.Select(unity => unity.transform.position).ToList();
        }
    }
}