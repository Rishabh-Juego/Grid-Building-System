using TGL.GridSystem.Buildings.UnSure;
using UnityEngine;

namespace TGL.GridSystem.Grid
{
    /// <summary>
    /// Data holder for each cell in the building grid.
    /// </summary>
    public class BuildingGridCell
    {
        private Building building;
        public void SetBuilding(Building building)
        {
            this.building = building;
        }
        public bool IsEmpty()
        {
            return building == null;
        }
    }
}