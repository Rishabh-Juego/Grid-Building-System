using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TGL.GridSystem.Buildings
{
    public class Wrapper : MonoBehaviour
    {
        [SerializeField] public List<AngledPosition> angledPositionData;
        [SerializeField] private  Vector3 initialRotation;
        private Vector3 initialPosition;
        private Dictionary<int, Vector3> angledPos;
        private Vector3 searchingPosition;

        private void Awake()
        {
            initialPosition = transform.localPosition;
            initialRotation = transform.localEulerAngles;
            if (ValidateAngledPositions())
            {
                CreatePositionDic();
            }
        }
        
        private bool ValidateAngledPositions()
        {
            if(angledPositionData == null || angledPositionData.Count == 0)
            {
                Debug.LogError($"angledPositionData is null or empty in Wrapper attached to {gameObject.name}. Please provide valid angled position data.", gameObject);
                return false;
            }
            HashSet<int> anglesSet = new HashSet<int>();
            // search for duplicate angles
            foreach (AngledPosition angledPosition in angledPositionData)
            {
                if (anglesSet.Contains(angledPosition.angleY))
                {
                    Debug.LogError($"Duplicate angle '{angledPosition.angleY}' found in angledPositionData of Wrapper attached to {gameObject.name}. Please ensure all angles are unique.", gameObject);
                    return false;
                }
                anglesSet.Add(angledPosition.angleY);
            }
            
            // Searching pairs of angles that are too close (within MAX_IGNORE_ANGLE)
            List<AngledPosition> sortedData = angledPositionData.OrderBy(ap => ap.angleY).ToList();
            for (int i = 0; i < sortedData.Count - 1; i++)
            {
                for (int j = i + 1; j < sortedData.Count; j++)
                {
                    int angleDifference = Mathf.Abs(sortedData[j].angleY - sortedData[i].angleY);
            
                    // Since angles wrap around at 360, we need to consider the circular nature
                    // The minimal difference between two angles is either the direct difference
                    // or 360 minus that difference
                    int circularDifference = Mathf.Min(angleDifference, 360 - angleDifference);
            
                    if (circularDifference <= AngledPosition.MAX_IGNORE_ANGLE)
                    {
                        Debug.LogError($"Angles '{sortedData[i].angleY}' and '{sortedData[j].angleY}' are too close (difference: {circularDifference} degrees) in angledPositionData of Wrapper attached to {gameObject.name}. Please ensure all angles are at least {AngledPosition.MAX_IGNORE_ANGLE + 1} degrees apart.", gameObject);
                        return false;
                    }
                }
            }
    
            return true;
        }

        private void CreatePositionDic()
        {
            angledPos = new Dictionary<int, Vector3>();
            foreach (AngledPosition angledPosition in angledPositionData)
            {
                angledPos.Add(angledPosition.angleY, angledPosition.positionAtAngleY);
            }
        }

        public void SetRotation(float angleToSetY)
        {
            while (angleToSetY > 360f)
            {
                angleToSetY -= 360f;
            }

            while (angleToSetY < 0f)
            {
                angleToSetY += 360f;
            }

            searchingPosition = initialPosition;
            foreach (KeyValuePair<int, Vector3> posData in angledPos)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(angleToSetY, posData.Key)) <= AngledPosition.MAX_IGNORE_ANGLE)
                {
                    searchingPosition = posData.Value;
                    break;
                }
            }
            transform.localPosition = searchingPosition;
            transform.localRotation = Quaternion.Euler(initialRotation.x, angleToSetY, initialRotation.z);     
        }
        
        [ContextMenu("Add Current Angle and Position to AngledPositionData")]
        private void AddCurrentAngleAndPosition()
        {
            int currentAngleY = Mathf.RoundToInt(transform.localEulerAngles.y);
            Vector3 currentPosition = transform.localPosition;
            angledPositionData.Add(new AngledPosition()
            {
                angleY = currentAngleY, 
                positionAtAngleY = currentPosition
            });
        }
    }
}