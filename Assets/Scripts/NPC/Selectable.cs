using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    public string SelectedLayerName = "Selection";
    private int selectedLayerId = 7;
    private int defaultLayerId = 0;
    public string DefaultLayerName = "Default";
    public bool Selected { get { return this.gameObject.layer == selectedLayerId; } set { SetSelected(value); } }

    private SkinnedMeshRenderer skinnedMeshRenderer;
    private MeshRenderer meshRenderer;
    
    public void SetSelected(bool selected)
    {
       
        if (selected)
        { 
            this.gameObject.layer = this.selectedLayerId;
            if (skinnedMeshRenderer != null)
            {
                skinnedMeshRenderer.gameObject.layer = this.selectedLayerId;
            }
            if (meshRenderer != null)
            {
                meshRenderer.gameObject.layer = this.selectedLayerId;
            }
            return;
        };
        this.gameObject.layer = this.defaultLayerId;
        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.gameObject.layer = this.defaultLayerId;
        }
        if(meshRenderer != null)
        {
            meshRenderer.gameObject.layer = this.defaultLayerId;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        selectedLayerId = LayerMask.NameToLayer(SelectedLayerName);
        defaultLayerId = LayerMask.NameToLayer(DefaultLayerName);
        skinnedMeshRenderer = this.GetComponentInChildren<SkinnedMeshRenderer>();
        meshRenderer = this.GetComponentInChildren<MeshRenderer>();
    }
}
