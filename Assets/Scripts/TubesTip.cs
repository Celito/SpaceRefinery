using UnityEngine;
using System.Collections.Generic;

public class TubesTip : MonoBehaviour
{
    public delegate void TubeCreated(Tube tube);
    public event TubeCreated OnTubeCreated;
    
    public GridManager.Direction direction = GridManager.Direction.BOTTOM;
    public uint numProjectionCubes = 1;
    public Material normalMaterial;
    public Material highlightedMaterial;

    private PointOfInterest _parentPOI;
    private MeshRenderer _meshRenderer;
    //private List<GameObject> _projectionCubes = new List<GameObject>();
    private bool _buildingStarted = false;
    //private int _selectedProjectionId;
    private Vector3 _initialPos;

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
        
        //for (var i = 0; i < numProjectionCubes; i++)
        //{
        //    var projectionCube = Instantiate(GridManager.instance.ProjectionCube);
        //    projectionCube.transform.parent = transform.parent;
        //    projectionCube.SetActive(false);
        //    var projectionCubeScript = projectionCube.GetComponent<ProjectionCube>();
        //    projectionCubeScript.SetProjectionId(i);
        //    //projectionCubeScript.OnMouseEntered += OnMouseEnterProjection;
        //    _projectionCubes.Add(projectionCube);
        //}
        _initialPos = transform.localPosition - PositionAdjustment(direction);
        SetDirection(direction);
    }

    //void OnMouseEnterProjection(int projectionId)
    //{  
    //    for(var i = 0; i < numProjectionCubes; i++)
    //    {
    //        var projectionCubeScript = _projectionCubes[i].GetComponent<ProjectionCube>();
    //        if (i <= projectionId)
    //        {
    //            projectionCubeScript.Highlight();
    //        }
    //        else
    //        {
    //            projectionCubeScript.RemoveHighlight();
    //        }
    //    }
    //    _selectedProjectionId = projectionId;
    //}

    void Update()
    {
        if(_buildingStarted && Input.GetButtonUp("Fire1"))
        {
            //Debug.Log("MOUSE RELEASED ON THE TUBE NUMBER " + (_selectedProjectionId + 1));
            //CreatCreateTubee the long part of the tube
            CreateTube();
        }
    }

    public void CreateTube()
    {
        Debug.Log("Creating a tube");
        //if (_selectedProjectionId > 0)
        //{
        //    var longTube = Instantiate(GridManager.instance.Tube4);
        //    longTube.transform.position = _parentPOI.transform.position + _initialPos +
        //        GridManager.instance.DirectionIncrement(direction);
        //    Tube longTubeScript = longTube.GetComponent<Tube>();
        //    longTubeScript.startDirection = GridManager.instance.OppositeDir(direction);
        //    longTubeScript.endDirection = direction;
        //    longTubeScript.SetSize(_selectedProjectionId);
        //}
        var endTube = Instantiate(GridManager.instance.Tube4);
        //foreach (var cube in _projectionCubes)
        //{
        //    cube.SetActive(false);
        //}
        endTube.transform.position = _parentPOI.transform.position + _initialPos +
            (GridManager.instance.DirectionIncrement(direction) /* * (_selectedProjectionId + 1)*/);
        Tube endTubeScript = endTube.GetComponent<Tube>();
        endTubeScript.startDirection = GridManager.instance.OppositeDir(direction);
        endTubeScript.endDirection = direction;
        endTubeScript.CreateExtensionTips();
        gameObject.SetActive(false);
        _buildingStarted = false;
        if (OnTubeCreated != null) OnTubeCreated(endTubeScript);
    }

    void OnMouseReleasedProjection(int projectionId)
    {
        Debug.Log("MOUSE RELEASED ON THE TUBE NUMBER " + (projectionId + 1));
    }

    void OnMouseDown()
    {
        //_buildingStarted = true;
        //foreach(var cube in _projectionCubes)
        //{
        //    cube.SetActive(true);
        //}
    }

    //void OnMouseEnter()
    //{
    //    GUIManager.instance.changeCursor(GUIManager.ADD_CURSOR);
    //    _meshRenderer.material = highlightedMaterial;
    //}

    //void OnMouseExit()
    //{
    //    GUIManager.instance.changeCursor(GUIManager.NORMAL_CURSOR);
    //    _meshRenderer.material = normalMaterial;
    //}

    public void SetDirection(GridManager.Direction direction)
    {
        var directionVector = GridManager.instance.DirectionIncrement(direction);
        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, directionVector, out hitInfo, numProjectionCubes))
        {
            numProjectionCubes = (uint)Mathf.Floor(hitInfo.distance);
            // use the Raycast to limit the num of projection cubes and get rid of the extra ones
        }
        //for (int i = 0; i < _projectionCubes.Count; i++)
        //{
        //    var projCube = _projectionCubes[i];
        //    projCube.transform.localPosition = 
        //        (GridManager.instance.DirectionIncrement(direction) * (i+1));
        //}
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
}
