using UnityEngine;
using System.Collections.Generic;

public class TubesTip : MonoBehaviour
{
    public enum TubeTipType
    {
        Start,
        Extension,
        Connection
    }

    public delegate void TubeCreated(Tube tube);
    public event TubeCreated OnTubeCreated;
    
    public GridManager.Direction direction = GridManager.Direction.BOTTOM;
    public uint numProjectionCubes = 1;
    public Material normalMaterial;
    public Material highlightedMaterial;

    private Structure _parentStructure;
    private MeshRenderer _meshRenderer;
    private bool _buildingStarted = false;
    private Vector3 _initialPos;

    private List<KeyValuePair<GridManager.Direction,Tube>> _possibleTubeConections;

    private TubeTipType _type = TubeTipType.Start;

    void Start()
    {
        bool structureParentFound = _parentStructure != null;
        var curParent = transform;
        while(!structureParentFound)
        {
            curParent = curParent.parent;
            if(curParent == null)
            {
                Debug.LogError("Missing a structure parent for a tubes begining");
                Debug.Break();
            }
            var parentGameObject = curParent.gameObject;

            _parentStructure = parentGameObject.GetComponent<Structure>();
            if (_parentStructure)
            {
                structureParentFound = true;
            }
        }

        _possibleTubeConections = new List<KeyValuePair<GridManager.Direction, Tube>>();

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

    public void CreateTube(int connectionIndex = 0)
    {
        if(_type == TubeTipType.Start)
        {
            var endTube = Instantiate(GridManager.instance.Tube4);
            endTube.transform.position = _parentStructure.transform.position + _initialPos +
                (GridManager.DirectionIncrement(direction) /* * (_selectedProjectionId + 1)*/);
            Tube endTubeScript = endTube.GetComponent<Tube>();
            endTubeScript.startDirection = GridManager.instance.OppositeDir(direction);
            endTubeScript.endDirection = direction;
            endTubeScript.CreateExtensionTips();
            if (_parentStructure)
            {
                endTubeScript.ConnectTo(_parentStructure);
            }
            gameObject.SetActive(false);
            _buildingStarted = false;
            if (OnTubeCreated != null) OnTubeCreated(endTubeScript);
        }
        else if(_type == TubeTipType.Extension)
        {
            (_parentStructure as Tube).ExtendTube(direction);
        }
        else /* _type == TubeTipType.Connection */
        {
            var connectionEntry = _possibleTubeConections[connectionIndex % _possibleTubeConections.Count];
            var connectionTube = connectionEntry.Value;
            var connectionDirection = connectionEntry.Key;
            if (_parentStructure is Tube)
            {
                // TODO: Combine the two tubes into one;
            }
            else
            {
                connectionTube.ExtendTube(GridManager.instance.OppositeDir(connectionDirection));
                connectionTube.SetTipDirection(GridManager.instance.OppositeDir(direction));
                connectionTube.ConnectTo(_parentStructure);
                // Delete the current tip;
                Destroy(gameObject);
            }
        }
    }

    public void SetTipType(TubeTipType type)
    {
        _type = type;
    }

    public TubeTipType GetTipType()
    {
        return _type;
    }

    public GridManager.Direction GetPossibleConnectionDirection(int index = 0)
    {
        if(_possibleTubeConections.Count > 0)
        {
            return _possibleTubeConections[index % _possibleTubeConections.Count].Key;
        }
        return 0;
    }

    public void SetParentStructure(Structure parent)
    {
        _parentStructure = parent;
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

    public void AddPossibleConnection(KeyValuePair<GridManager.Direction, Tube> directionAndTube)
    {
        if(!_possibleTubeConections.Contains(directionAndTube))
        {
            _possibleTubeConections.Add(directionAndTube);
            SetTipType(TubeTipType.Connection);
        }
    }

    public void RemovePossibleConnection(Tube tube)
    {
        foreach(var pair in _possibleTubeConections)
        {
            if(pair.Value == tube)
            {
                _possibleTubeConections.Remove(pair);
                break;
            }
        }
        if(_possibleTubeConections.Count == 0)
        {
            SetTipType(_parentStructure is Tube ? TubeTipType.Extension : TubeTipType.Start);
        }
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
