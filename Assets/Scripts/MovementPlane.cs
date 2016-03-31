using UnityEngine;
using System.Collections;

public class MovementPlane : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnMouseUp()
    {
        Debug.Log("plane click");
    }

    void OnMouseEnter()
    {
        GUIManager.instance.changeCursor(GUIManager.DRAGBLE_CURSOR);
    }

    void OnMouseExit()
    {
        GUIManager.instance.changeCursor(GUIManager.NORMAL_CURSOR);
    }
}
