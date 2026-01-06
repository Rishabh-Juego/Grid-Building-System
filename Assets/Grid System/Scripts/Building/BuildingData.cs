using UnityEngine;

namespace TGL.GridSystem.Buildings
{
    /// <summary>
    /// ScriptableObject holding data for a building type in the grid system.
    /// </summary>
    [CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/Data/BuildingData")]
    public class BuildingData : ScriptableObject
    {
        [field: SerializeField] public string BuildingName {get; private set;}
        [field: SerializeField] public string Description {get; private set;}
        [SerializeField] public int cost {get; private set;}
        [field: SerializeField] public BuildingModel Model {get; private set;}
    }
}