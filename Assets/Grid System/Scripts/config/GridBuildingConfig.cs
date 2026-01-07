using UnityEngine;

namespace TGL.GridSystem.config
{
    /// <summary>
    /// Plan to convert this to ScriptableObject later using <see cref="GridBuildingConfig2"/>
    /// </summary>
    public static class GridBuildingConfig
    {
        public const float cellSize = 5f;
        public const int rotateStep = 90;
        // gridLayerMask has name "Grid" and value 6 in the layer table
        //public static LayerMask GridLayerMask = 1 << 6;
        public static LayerMask gridLayerMask = LayerMask.GetMask("Grid");
    }
}