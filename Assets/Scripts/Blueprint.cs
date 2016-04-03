using UnityEngine;
using System.Collections;

public class Blueprint : MonoBehaviour {

    public Material buildingEnabledMaterial;
    public Material buildingBlockedMaterial;

    private GameObject _currTip;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void SetEnableToBuild(bool value)
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            Renderer childRenderer = child.GetComponent<Renderer>();
            childRenderer.material = value ? buildingEnabledMaterial : buildingBlockedMaterial; 
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "TubeTip")
        {
            _currTip = other.gameObject;
            SetEnableToBuild(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "TubeTip")
        {
            _currTip = null;
            SetEnableToBuild(false);
        }
    }
    
    void OnMouseDown()
    {
        if(_currTip && _currTip.activeSelf)
        {

            Debug.Log("Trying to extend a tube");
            _currTip.SendMessage("CreateTube");
        }
    }
}
