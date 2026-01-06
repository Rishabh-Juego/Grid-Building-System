using UnityEngine;

namespace TGL.GridSystem.config
{
    /// <summary>
    /// For Future Configurations related to Building Grid System
    /// </summary>
    [CreateAssetMenu(fileName = "GridBuildingConfig2", menuName = "Scriptable Objects/config/GridBuildingConfig2")]
    public class GridBuildingConfig2 : ScriptableObject
    {
        [Tooltip("Width and height in Unity Units"), Range(1, 10)]
        public int cellSize = 5;

        [Tooltip("Degrees we move for a single rotation"), Range(0, 180)]
        public int rotateStep = 90;
        
        /// <summary>
        /// Positive and Negative Materials for Grid Cell Visualization
        /// </summary>
        [Tooltip("The material to use when preview allows for building placement")] public Material positiveMaterial;
        [Tooltip("The material to use then preview block for building placement")] public Material negativeMaterial;

        /// <summary>
        /// The Collider layer for the grid
        /// </summary>
        [Tooltip("The Collider Layer for the grid")]public LayerMask gridLayerMask;
    }
}