using UnityEngine;
using System.Collections.Generic;

public class TubesTip : MonoBehaviour
{
    public enum TubeTipType
    {
        Start,
        Extension,
        Conection
    }

    public delegate void TubeCreated(Tube tube);
    public event TubeCreated OnTubeCreated;
    
    public GridManager.Direction direction = GridManager.Direction.BOTTOM;
    public uint numProjectionCubes = 1;
    public Material normalMaterial;
    public Material highlightedMaterial;

    private PointOfInterest _parentPOI;
    private MeshRenderer _meshRenderer;
    private bool _buildingStarted = false;
    private Vector3 _initialPos;

    private Tube _parentTube;

    private TubeTipType _type = TubeTipType.Start;

    void Start()
    {
        bool poiParentFound = false;
        var curParent = transform;
        while(!poiParentFound)
        {
            curParent = curParent.parent;
            if(curParent == null)
            {
                Debug.LogError("Missing a POI parent for a tubes begining");
                Debug.Break();
            }
            var parentGameObject = curParent.gameObject;
            _parentPOI = parentGameObject.GetComponent<PointOfInterest>();
            if (_parentPOI)
            {
                poiParentFound = true;
            }
        }

        _meshRenderer = GetComponent<MeshRenderer>();

        _initialPos = transform.localPosition - PositionAdjustment(direction);
        SetDirection(direction);
    }

    void Update()
    {
        if(_buildingStarted && Input.GetButtonUp("Fire1"))
        {
            CreateTube();
        }
    }

    public void CreateTube()
    {
        if(_type == TubeTipType.Start)
        {
            Debug.Log("Creating a tube");
            var endTube = Instantiate(GridManager.instance.Tube4);
            endTube.transform.position = _parentPOI.transform.position + _initialPos +
                (GridManager.DirectionIncrement(direction) /* * (_selectedProjectionId + 1)*/);
            Tube endTubeScript = endTube.GetComponent<Tube>();
            endTubeScript.startDirection = GridManager.instance.OppositeDir(direction);
            endTubeScript.endDirection = direction;
            endTubeScript.CreateExtensionTips();
            gameObject.SetActive(false);
            _buildingStarted = false;
            if (OnTubeCreated != null) OnTubeCreated(endTubeScript);
        }
        else if(_type == TubeTipType.Extension)
        {
            Debug.Log("Extending a tube");
            _parentTube.ExtendTube(direction);
        }
    }

    public void SetType(TubeTipType type)
    {
        _type = type;
    }

    public void SetParentTube(Tube parentTube)
    {
        _parentTube = parentTube;
    }

    void OnMouseReleasedProjection(int projectionId)
    {
        Debug.Log("MOUSE RELEASED ON THE TUBE NUMBER " + (projectionId + 1));
    }

    public void MoveTo(Vector3 pos)
    {
        _initialPos = pos;
        SetDirection(direction);
    }

    public void SetDirection(GridManager.Direction direction)
    {
        var directionVector = GridManager.DirectionIncrement(direction);
        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, directionVector, out hitInfo, numProjectionCubes))
        {
            numProjectionCubes = (uint)Mathf.Floor(hitInfo.distance);
            // use the Raycast to limit the num of projection cubes and get rid of the extra ones
        }
        transform.localPosition = PositionAdjustment(direction) + _initialPos;
        switch (direction)
        {
            case GridManager.Direction.TOP:
                transform.localEulerAngles = new Vector3(0f, 0f, 0f);
                break;
            case GridManager.Direction.BOTTOM:
                transform.localEulerAngles = new Vector3(180f, 0f, 0f);
                break;
            case GridManager.Direction.FRONT:
                transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                break;
            case GridManager.Direction.BACK:
                transform.localEulerAngles = new Vector3(-90f, 0f, 0f);
                break;
            case GridManager.Direction.RIGHT:
                transform.localEulerAngles = new Vector3(0f, 0f, 270f);
                break;
            case GridManager.Direction.LEFT:
                transform.localEulerAngles = new Vector3(0, 0f, 90f);
                break;
        }
        this.direction = direction;
    }

    private Vector3 PositionAdjustment(GridManager.Direction dir)
    {
        switch(dir)
        {
            case GridManager.Direction.TOP:
                return new Vector3(0f, .5f, 0f);
            case GridManager.Direction.BOTTOM:
                return new Vector3(0f, -.5f, 0f);
            case GridManager.Direction.FRONT:
                return new Vector3(0f, 0f, .5f);
            case GridManager.Direction.BACK:
                return new Vector3(0f, 0f, -.5f);
            case GridManager.Direction.RIGHT:
                return new Vector3(.5f, 0f, 0f);
            case GridManager.Direction.LEFT:
                return new Vector3(-.5f, 0f, 0f);
        }
        return Vector3.zero;
    }

    void OnValidate()
    {
        _initialPos = transform.localPosition - PositionAdjustment(direction);
        SetDirection(direction);
    }
}
