using UnityEngine;
using System.Collections;

public class Blueprint : MonoBehaviour
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected void SetEnableToBuild(bool value)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            Renderer childRenderer = child.GetComponent<Renderer>();
            childRenderer.material = 
                value ? 
                    GridManager.instance.BuildingEnabledMaterial : 
                    GridManager.instance.BuildingBlockedMaterial; 
        }
    }

    virtual public void MoveBlueprintTo(Vector3 pos)
    {
        transform.position = pos;
    }
}
