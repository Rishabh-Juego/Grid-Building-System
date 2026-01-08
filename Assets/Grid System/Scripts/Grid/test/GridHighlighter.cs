using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace TGL.GridSystem.Grid
{
    public class GridHighlighter : MonoBehaviour
    {
        /// <summary>
        /// The Material used to render the grid highlights Should be of the correct shader type
        /// </summary>
        public Material gridMaterial;
        /// <summary>
        /// The Width and Height of the grid in cells
        /// </summary>
        public Vector2Int gridSize; 

        /// <summary>
        /// Runtime texture used to highlight grid cells
        /// </summary>
        private Texture2D _highlightTex;
        /// <summary>
        /// Pixel data for the highlight texture, used for fast updates
        /// </summary>
        private NativeArray<Color32> _pixelData;
        /// <summary>
        /// Reference Name of the Texture2D variable in the shader for the highlight map
        /// </summary>
        private static readonly int HighlightMapProperty = Shader.PropertyToID("_HighlightMap");
        
        [Space(15), Header("TEsTING")]
        public List<Vector2Int> highlightedCells;

        void Start()
        {
            SetHighlightTexture();
        }
        
        [ContextMenu("UpdateHightlights")]
        private void UpdateHightlights()
        {
            if (_highlightTex == null || gridMaterial.GetTexture(HighlightMapProperty) != _highlightTex)
            {
                SetHighlightTexture();
            }
            UpdateHighlights(highlightedCells);
        }
        
        private void SetHighlightTexture()
        {
            // Initialize texture
            _highlightTex = new Texture2D(gridSize.x, gridSize.y, TextureFormat.RGBA32, false);
            _highlightTex.filterMode = FilterMode.Point;
            _highlightTex.wrapMode = TextureWrapMode.Clamp;

            gridMaterial.SetTexture(HighlightMapProperty, _highlightTex);

            // Grab the raw data pointer
            _pixelData = _highlightTex.GetRawTextureData<Color32>();
        }

        public void UpdateHighlights(List<Vector2Int> highlightedGridCells)
        {
            // 1. Clear the data (using NativeArray is very fast)
            for (int i = 0; i < _pixelData.Length; i++)
            {
                _pixelData[i] = new Color32(0, 0, 0, 0);
            }

            // 2. Set specific cells
            foreach (var cell in highlightedGridCells)
            {
                int index = cell.y * gridSize.x + cell.x;
                if (index >= 0 && index < _pixelData.Length)
                {
                    _pixelData[index] = new Color32(255, 255, 255, 255);
                }
            }

            // 3. Upload to GPU
            _highlightTex.Apply();
        }
    }
}
