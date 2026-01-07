using System;
using System.Collections.Generic;
using System.Linq;
using TGL.GridSystem.Buildings;
using TGL.GridSystem.config;
using TGL.GridSystem.Grid;
using UnityEngine;

namespace TGL.GridSystem.placements
{
    public class BuildingSystem : MonoBehaviour
    {
        // skipping building selection Menu System for now
        [Header("Buildings"), SerializeField] private BuildingData buildingData1;
        [SerializeField] private BuildingData buildingData2;
        [SerializeField] private BuildingData buildingData3;
        
        [Header("Scene References"), SerializeField] private Camera cam;
        [SerializeField] private BuildingGrid buildingGrid;
        
        [Header("Prefabs"), SerializeField] private BuildingPreview buildingPreviewPrefab;
        [SerializeField] private Building buildingPrefab;
        
        private BuildingPreview buildingPreviewInstance;
        Vector3 mouseWorldPosition;
        private List<Vector3> buildPositions;
        private Ray mouseRay;

        private List<float> xs, zs;
        float centerX, centerZ;

        private void Update()
        {
            if(buildingPreviewInstance == null && 
                !Input.GetKeyDown(KeyCode.Alpha1) && 
                !Input.GetKeyDown(KeyCode.Alpha2) && 
                !Input.GetKeyDown(KeyCode.Alpha3))
                return;
            
            mouseWorldPosition = GetMouseGridWorldPosition();
            if (buildingPreviewInstance != null)
            {
                HandlePreview(mouseWorldPosition);
            }
            else if(!Mathf.Approximately(mouseWorldPosition.magnitude, 0))
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Debug.Log($"Creating building preview for {nameof(buildingData1)} at - MouseWorldPos: {mouseWorldPosition}");
                    buildingPreviewInstance = CreateBuildingPreview(buildingData1, mouseWorldPosition);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Debug.Log($"Creating building preview for {nameof(buildingData2)} at - MouseWorldPos: {mouseWorldPosition}");
                    buildingPreviewInstance = CreateBuildingPreview(buildingData2, mouseWorldPosition);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    Debug.Log($"Creating building preview for {nameof(buildingData3)} at - MouseWorldPos: {mouseWorldPosition}");
                    buildingPreviewInstance = CreateBuildingPreview(buildingData3, mouseWorldPosition);
                }
            }
            else
            {
                Debug.Log($"Cannot process any input as mouse is not over the grid area - MouseWorldPos: {mouseWorldPosition}");
            }
        }

        private Vector3 GetMouseGridWorldPosition()
        {
            // GridBuildingConfig.gridLayerMask can be used here for better accuracy
            mouseRay = cam.ScreenPointToRay(Input.mousePosition);
            if (GridBuildingConfig.gridLayerMask != 0) // using colliders on the grid at specified layer mask
            {
                if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, maxDistance: Mathf.Infinity, GridBuildingConfig.gridLayerMask))
                {
                    return hitInfo.point;
                }
            }
            else
            {
                // Debug.LogWarning($"Did we not create a collider and assign it to the layer mask {GridBuildingConfig.gridLayerMask}? we are assuming Building system as the start point and this may give wrong results");
                // create a plane at y = transform.position.y with normal pointing up
                Plane gridPlane = new Plane(Vector3.up, transform.position);
                // raycast against the plane, if we can get a hit, return the point of intersection
                if (gridPlane.Raycast(mouseRay, out float distance))
                {
                    return mouseRay.GetPoint(distance);
                }
            }

            return Vector3.zero;
        }
        
        private BuildingPreview CreateBuildingPreview(BuildingData data, Vector3 position)
        {
            BuildingPreview preview = Instantiate(buildingPreviewPrefab, position, Quaternion.identity);
            preview.Setup(data);
            return preview;
        }

        private void HandlePreview(Vector3 mousePos)
        {
            buildingPreviewInstance.transform.position = buildingGrid.GetCellLowerEdgePos(mousePos);
            buildPositions = buildingPreviewInstance.Model.GetAllBuildingShapePositions();
            if(buildPositions.Count == 0)
            {
                Debug.LogError("No building shape positions found in the building model", buildingPreviewInstance);
                #if UNITY_EDITOR
                    Debug.Break();
                #endif
                return;
            }
            bool canBuild = buildingGrid.CanBuild(buildPositions, false);
            if(Input.GetKeyDown(KeyCode.R)) // 'R' key on Keyboard
            {
                buildingPreviewInstance.Model.RotateModel(GridBuildingConfig.rotateStep);
            }
            if (canBuild)
            {
                //buildingPreviewInstance.transform.position = GetSnappedCenterPosition(buildPositions);
                buildingPreviewInstance.ChangeState(BuildingPreview.BuildingPreviewState.POSITIVE);
                if (Input.GetMouseButtonDown(0)) // mouse Down
                {
                    buildingGrid.CanBuild(buildPositions, true);
                    // place the building
                    Debug.Log($"Placing the building at - preview pos: {buildingPreviewInstance.transform.position}");
                    PlaceBuilding(buildPositions);
                }
            }
            else
            {
                buildingPreviewInstance.ChangeState(BuildingPreview.BuildingPreviewState.NEGATIVE);
            }
        }

        private Vector3 GetSnappedCenterPosition(List<Vector3> allbuildingPositions)
        {
            xs = allbuildingPositions.Select(p => p.x).ToList();
            zs = allbuildingPositions.Select(p => p.z).ToList();
            if (xs.Count == 1)
            {
                centerX = xs[0];
            }
            else
            {
                centerX = (xs.Min() + xs.Max()) / 2.0f;
            }


            if (zs.Count == 1)
            {
                centerZ = zs[0];
            }
            else
            {
                centerZ = (zs.Min() + zs.Max()) / 2.0f;
            }
            FindNearestGridPoint(ref centerX, ref centerZ, GridBuildingConfig.cellSize);
            // centerX += GridBuildingConfig.cellSize / 2.0f;
            // centerZ += GridBuildingConfig.cellSize / 2.0f;
            
            return new Vector3(centerX, transform.position.y, centerZ);
        }
        
        private void FindNearestGridPoint(ref float centerX, ref float centerZ, float cellSize)
        {
            centerX = Mathf.Round(centerX / cellSize) * cellSize;
            centerZ = Mathf.Round(centerZ / cellSize) * cellSize;
        }
        

        private void PlaceBuilding(List<Vector3> buildPosition)
        {
            Building building = Instantiate(buildingPrefab, buildingPreviewInstance.transform.position, buildingPreviewInstance.transform.rotation);
            building.Setup(buildingPreviewInstance.Data, Mathf.RoundToInt(buildingPreviewInstance.Model.RotationY));
            buildingGrid.SetBuilding(building, buildPosition);
            Destroy(buildingPreviewInstance.gameObject);
            buildingPreviewInstance = null;
        }
        
        private void OnDrawGizmos()
        {
            if (mouseRay.direction != Vector3.zero)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(mouseRay.origin, mouseRay.direction * 100f);
            }
        }
    }
}