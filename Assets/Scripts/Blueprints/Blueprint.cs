using UnityEngine;
using System.Collections;

public class Blueprint : MonoBehaviour
{
    protected bool _isEnableToBuild;

	// Use this for initialization
	void Start () {
        _isEnableToBuild = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected void SetEnableToBuild(bool value)
    {
        _isEnableToBuild = value;
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
