using UnityEngine;
using System.Collections;

public class Meter : MonoBehaviour {

    private GameObject _content;
    private Structure _parentStructure;

	// Use this for initialization
	void Start () {
        _content = transform.Find("Content").gameObject;
        bool structureParentFound = _parentStructure != null;
        var curParent = transform;
        while (!structureParentFound)
        {
            curParent = curParent.parent;
            if (curParent == null)
            {
                Debug.LogError("Missing a structure parent for a meter");
                Debug.Break();
            }
            var parentGameObject = curParent.gameObject;

            _parentStructure = parentGameObject.GetComponent<Structure>();
            if (_parentStructure)
            {
                structureParentFound = true;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if(_parentStructure.CurrPresure > 0.05)
        {
            var currPresure = Mathf.Clamp((float)_parentStructure.CurrPresure, 0, 1);
            _content.SetActive(true);
            _content.transform.localScale = new Vector3(_content.transform.localScale.x, currPresure * 0.96f, _content.transform.localScale.z);
            _content.transform.localPosition = new Vector3(_content.transform.localPosition.x, -1 + currPresure, _content.transform.localPosition.z);
        }
        else if(_content.activeSelf)
        {
            _content.SetActive(false);
        }
	}
}
