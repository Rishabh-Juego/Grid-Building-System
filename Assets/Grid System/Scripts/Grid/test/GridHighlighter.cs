using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

namespace TGL.GridSystem.Grid
{
    [RequireComponent(typeof(Renderer))]
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
        private static readonly string HighlightMapPropertyName = "_HighlightMap";
        private static readonly int HighlightMapProperty = Shader.PropertyToID(HighlightMapPropertyName);
        
        private static readonly string HighlightColorPropertyName = "_HighlightFloorColor";
        private static readonly int HighlightColorProperty = Shader.PropertyToID(HighlightColorPropertyName);
        
        
        [SerializeField] private List<Vector2Int> highlightedCells = new List<Vector2Int>();
        [SerializeField] private Color highlightColor = Color.white; 
        private Renderer _renderer;

        
        private void Awake()
        {
            _renderer = GetComponent(typeof(Renderer)) as Renderer;
            if (_renderer == null)
            {
                Debug.LogError($"Could not find Renderer component on {gameObject.name}. Disabling GridHighlighter script.", this);
                return;
            }
            gridMaterial = _renderer.sharedMaterial;
            if (!gridMaterial.HasTexture(HighlightMapProperty))
            {
                Debug.LogError($"Could not find HighlightMap property('{HighlightMapPropertyName}') on Shader attached to {gameObject.name}", gridMaterial);
                return;
            }

            if (!gridMaterial.HasColor(HighlightColorProperty))
            {
                Debug.LogError($"Could not find HighlightColor property('{HighlightColorPropertyName}') on Shader attached to {gameObject.name}", gridMaterial);
                return;
            }
            else
            {
                highlightColor.a = 0;
            }
        }

        private void SetupHighlightTexture()
        {
            // Initialize texture
            _highlightTex = new Texture2D(gridSize.x, gridSize.y, TextureFormat.RGBA32, false);
            _highlightTex.filterMode = FilterMode.Point;
            _highlightTex.wrapMode = TextureWrapMode.Clamp;

            gridMaterial.SetTexture(HighlightMapProperty, _highlightTex);

            // Grab the raw data pointer
            _pixelData = _highlightTex.GetRawTextureData<Color32>();
            gridMaterial.SetColor(HighlightColorProperty, highlightColor);
        }
        
        public void Setup(int width, int height)
        {
            gridSize = new Vector2Int(width, height);
            UpdateHighlightTexture();
        }
        
        public void SetHighlightedCells(List<Vector2Int> highlightedGridCells, Color showColor)
        {
            highlightedCells = highlightedGridCells;
            highlightColor = showColor;
            UpdateHighlightTexture();
        }
        
        private void UpdateHighlightTexture()
        {
            if (gridSize.magnitude == 0)
            {
                Debug.LogError($"If grid size is not set on GridHighlighter attached to {gameObject.name}, please call Setup(width, height) before updating highlights.", this);
                return;
            }
            
            if (_highlightTex == null || gridMaterial.GetTexture(HighlightMapProperty) != _highlightTex)
            {
                SetupHighlightTexture();
            }

            if (gridMaterial.GetColor(HighlightColorProperty) != highlightColor)
            {
                gridMaterial.SetColor(HighlightColorProperty, highlightColor);
            }
            
            UpdateHighlights();
        }

        private void UpdateHighlights()
        {
            // 1. Clear the data (using NativeArray is very fast)
            for (int i = 0; i < _pixelData.Length; i++)
            {
                _pixelData[i] = new Color32(0, 0, 0, 0);
            }

            // 2. Set specific cells
            foreach (var cell in highlightedCells)
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
