using UnityEngine;
using System.Collections;
using DG.Tweening;

public class MainCamera : MonoBehaviour
{
    public static MainCamera instance = null;

    public PointOfInterest initialPOI;
    public float initalDistance = 10f;
    public float maxDistance = 20f;
    public float minDistance = 5f;

    private Transform _cameraBoomTransform;

    private PointOfInterest _currPOI;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start ()
    {
        _cameraBoomTransform = transform.parent;

        // put the camera boom at the initial object
        //_cameraBoomTransform.parent = initialPOI.transform;

        // set the initial distance camera distance from the object
        transform.localPosition.Set(0f, 0f, initalDistance);

        initialPOI.Select();

        _currPOI = initialPOI;
    }
	
	// Update is called once per frame
	void Update ()
    {
	    // TODO: zoom in and out with the mouse whell
	}

    public void SetNewPOI(PointOfInterest poi)
    {

        // move the camera to the new POI;
        if (_currPOI)
        {
            _currPOI.Deselect();
        }

        _cameraBoomTransform.parent = poi.transform;
        _cameraBoomTransform.localPosition = Vector3.zero;
        
        _currPOI = poi;

        poi.Select();

        // TODO: create a tween between the old position to the new camera position;
    }

    public void RotateY(bool counter = false)
    {

        var eulerAngles = _cameraBoomTransform.rotation.eulerAngles;
        eulerAngles.y += counter ? -90 : 90;
        _cameraBoomTransform.DORotate(eulerAngles, 0.5f);
        
        
        //_cameraBoomTransform.Rotate(Vector3.up, counter? -90 : 90f, Space.World);
    }
}
