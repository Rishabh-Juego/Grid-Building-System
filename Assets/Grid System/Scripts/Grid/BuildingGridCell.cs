using TGL.GridSystem.Buildings.UnSure;
using UnityEngine;

namespace TGL.GridSystem.Grid
{
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