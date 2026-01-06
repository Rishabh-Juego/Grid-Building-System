using UnityEngine;

namespace TGL.GridSystem.Buildings
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "Scriptable Objects/Data/BuildingData")]
    public class BuildingData : ScriptableObject
    {
        [field: SerializeField] public string BuildingName {get; private set;}
        [field: SerializeField] public string Description {get; private set;}
        [SerializeField] public int cost {get; private set;}
        [field: SerializeField] public BuildingModel Model {get; private set;}
    }
}