using UnityEngine;
using TGL.GridSystem.Buildings;
using System.Collections.Generic;

namespace TGL.GridSystem.placements
{
	public class BuildingPreview : MonoBehaviour
	{
	    public enum BuildingPreviewState
	    {
	        NONE = 0,
	        POSITIVE = 1,
	        NEGATIVE = 2
    	}
    	[SerializeField] private Material positiveMaterial;
    	[SerializeField] private Material negativeMaterial;
    	public BuildingPreviewState previewState{get; private set;} = BuildingPreviewState.NEGATIVE;
    	public BuildingData Data {get; private set;}
	    
    	// Colliders and renderers
    	private List<Collider> colliders = new ();
    	private List<Renderer> renderers = new ();
	    
    	public void Setup(BuildingData data)
    	{
        	Data = data;
        	BuildingModel model = Instantiate(Data.Model, transform.position, Quaternion.identity, transform);
        	renderers.AddRange(model.GetComponentsInChildren<Renderer>());
        	colliders.AddRange(model.GetComponentsInChildren<Collider>());
			foreach (var col in colliders)
			{
				col.enabled = false;
			}
    	}
	}
}
