using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TGL.GridSystem.Buildings
{
    /// <summary>
    /// Marker class for each unit of a building's shape in the grid system.
    /// </summary>
    public class BuildingShapeUnit : MonoBehaviour
    {
#if UNITY_EDITOR
        private const float CIRCLE_RADIUS = 0.5f;
        private static readonly Color gizmoColor = Color.black;

        private void OnDrawGizmos()
        {
            Handles.color = gizmoColor;
            Handles.DrawSolidDisc(transform.position, Vector3.up, CIRCLE_RADIUS);
        }
#endif
    }
}