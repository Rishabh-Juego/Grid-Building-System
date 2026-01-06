using UnityEngine;

namespace TGL.GridSystem.Buildings.UnSure
{
    /// <summary>
    /// Details and references for a building instance in the grid system.
    /// This is the building placed in the world, instantiated from BuildingData.
    /// </summary>
    public class Building : MonoBehaviour
    {
        private string Description => data.Description;
        
        private BuildingModel model;
        private BuildingData data;
        public void Setup(BuildingData data, int rotationAngle)
        {
            this.data = data;
            model = Instantiate(data.Model, transform.position, Quaternion.Euler(0, rotationAngle, 0), transform);
        }
    }
}
