using UnityEngine;
using TGL.GridSystem.Buildings;
using System.Collections.Generic;

namespace TGL.GridSystem.placements
{
	/// <summary>
	/// Shows a preview of the building to be placed.
	/// Changes material based on whether placement is valid or not.
	/// Looks like the ghost of the building to be placed.
	/// </summary>
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
	    public BuildingModel Model => model;
	    
    	// Colliders and renderers
    	private List<Collider> colliders = new ();
    	private List<Renderer> renderers = new ();
	    private Dictionary<Renderer, Material[]> originalMaterials = new ();
	    private BuildingModel model;
	    
    	public void Setup(BuildingData data)
    	{
        	Data = data;
        	model = Instantiate(Data.Model, transform.position, Quaternion.identity, transform);
        	renderers.AddRange(model.GetComponentsInChildren<Renderer>());
        	colliders.AddRange(model.GetComponentsInChildren<Collider>());
			foreach (var col in colliders)
			{
				col.enabled = false;
			}
			SaveOriginalMaterials();
			SetPreviewMaterial(previewState);
    	}

	    private void SaveOriginalMaterials()
	    {
		    originalMaterials = new Dictionary<Renderer, Material[]>();
		    foreach (Renderer rend in renderers)
		    {
			    originalMaterials.Add(rend, rend.materials);
		    }
	    }

	    private void SetPreviewMaterial(BuildingPreviewState newState)
	    {
		    Material stateMat = null;
		    switch (newState)
		    {
			    case BuildingPreviewState.NEGATIVE:
				    stateMat = negativeMaterial;
				    break;
			    case BuildingPreviewState.POSITIVE:
				    stateMat = positiveMaterial;
				    break;
			    case BuildingPreviewState.NONE:
			    default:
				    Debug.LogError($"The state {newState} is not handled");
				    return;
		    }

		    foreach (Renderer rend in renderers)
		    {
			    Material[] mats = new Material[rend.sharedMaterials.Length];
			    for(int i=0; i < mats.Length; i++)
			    {
				    mats[i] = stateMat;
			    }
			    rend.materials = mats;
		    }
	    }

	    public void ChangeState(BuildingPreviewState newState)
	    {
		    if(newState == previewState) return;
		    previewState = newState;
		    SetPreviewMaterial(previewState);
	    }
	    
	    public void RotatePreview(int rotationStep)
	    {
		    if(Data == null || Data.Model == null) return;
		    model.RotateModel(rotationStep);
	    }
	}
}
