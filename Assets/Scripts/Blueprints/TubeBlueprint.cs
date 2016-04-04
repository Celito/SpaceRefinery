using UnityEngine;
using System.Collections;

public class TubeBlueprint : Blueprint
{

    private GameObject _currTip;

    // Use this for initialization
    void Start ()
    {
	
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
        if (_currTip && _currTip.activeSelf)
        {

            Debug.Log("Trying to extend a tube");
            _currTip.SendMessage("CreateTube");
        }
    }
}
