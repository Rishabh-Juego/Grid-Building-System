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
        
        public Material positiveMaterial;
        public Material negativeMaterial;
    }
}