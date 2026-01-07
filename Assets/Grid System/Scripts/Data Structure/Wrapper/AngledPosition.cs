using UnityEngine;

namespace TGL.GridSystem.Buildings
{
    [System.Serializable]
    public class AngledPosition 
    {
        public const int MAX_IGNORE_ANGLE = 10;
        public int angleY;
        public Vector3 positionAtAngleY;
    }
}