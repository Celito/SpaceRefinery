using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tube : Structure
{

    public GridManager.Direction startDirection = GridManager.Direction.FRONT;
    public GridManager.Direction endDirection = GridManager.Direction.BACK;

    private GameObject _body;
    private GameObject _connection1;
    private GameObject _connection2;

    private List<TubesTip> _extensionTips;

    private Vector2 _currConexion = new Vector2(3, 4); // the prefeb is positioned at the direction 3 by default;
    private bool _reversed;
    private int _size = 1;

    private List<SectionInfo> _sections;
    private Vector3 _globalEndPosition;
    private List<TubesTip> _currPossibleConnections;
    private List<BoxCollider> _colliders;

    private static int uniqueTubeIdIter = 0;

    public struct DirectionInfo
    {
        public int id;
        public Vector3 rotation;
        public Vector3 position;
    }

    class SectionInfo
    {
        public uint length;
        public GameObject body;
        public GameObject startTip;
        public GameObject endTip;
        public GridManager.Direction startDir;
        public GridManager.Direction endDir;
        public Vector3 initialPosition;
        public bool isCurve = false;
    }

    public static Dictionary<Vector2, DirectionInfo> BodyDirTable = new Dictionary<Vector2, DirectionInfo>()
    {
        { new Vector2(1, 6), new DirectionInfo { id =  1, rotation = new Vector3( 90,   0,   0) } },
        { new Vector2(2, 5), new DirectionInfo { id =  2, rotation = new Vector3(  0,  90,   0) } },
        { new Vector2(3, 4), new DirectionInfo { id =  3, rotation = new Vector3(  0,   0,   0) } },
        { new Vector2(1, 2), new DirectionInfo { id =  7, rotation = new Vector3(270,   0,   0) } },
        { new Vector2(1, 3), new DirectionInfo { id =  5, rotation = new Vector3(  0,   0,  90) } },
        { new Vector2(1, 4), new DirectionInfo { id =  6, rotation = new Vector3(270,  90,   0) } },
        { new Vector2(1, 5), new DirectionInfo { id =  4, rotation = new Vector3(270, 180,   0) } },
        { new Vector2(2, 3), new DirectionInfo { id = 11, rotation = new Vector3(  0,   0,   0) } },
        { new Vector2(2, 4), new DirectionInfo { id = 13, rotation = new Vector3(  0,  90,   0) } },
        { new Vector2(2, 6), new DirectionInfo { id = 15, rotation = new Vector3( 90,   0,   0) }},
        { new Vector2(3, 5), new DirectionInfo { id =  8, rotation = new Vector3(  0, 270,   0) } },
        { new Vector2(3, 6), new DirectionInfo { id = 12, rotation = new Vector3(  0,   0, 270) } },
        { new Vector2(4, 5), new DirectionInfo { id =  9, rotation = new Vector3(  0, 180,   0) } },
        { new Vector2(4, 6), new DirectionInfo { id = 14, rotation = new Vector3( 90,  90,   0) } },
        { new Vector2(5, 6), new DirectionInfo { id = 10, rotation = new Vector3( 90, 180,   0) } }
    };

    public static Dictionary<GridManager.Direction, DirectionInfo> TipDirTable =
        new Dictionary<GridManager.Direction, DirectionInfo>()
    {
        { GridManager.Direction.BOTTOM, new DirectionInfo { id = 1, rotation = new Vector3( 90,  0,  0), position = new Vector3( 0, -.5f, 0)} },
        { GridManager.Direction.RIGHT , new DirectionInfo { id = 2, rotation = new Vector3(  0, 90,  0), position = new Vector3( .5f, 0, 0)} },
        { GridManager.Direction.BACK  , new DirectionInfo { id = 3, rotation = new Vector3(  0,  0,  0), position = new Vector3( 0, 0, -.5f) } },
        { GridManager.Direction.FRONT , new DirectionInfo { id = 4, rotation = new Vector3(  0,  0,  0), position = new Vector3( 0, 0, .5f) } },
        { GridManager.Direction.LEFT  , new DirectionInfo { id = 5, rotation = new Vector3(  0, 90,  0), position = new Vector3( -.5f, 0, 0) } },
        { GridManager.Direction.TOP   , new DirectionInfo { id = 6, rotation = new Vector3( 90,  0,  0), position = new Vector3( 0, .5f, 0) } },
    };


    // Use this for initialization
    override protected void VirtualAwake()
    {
        _body = transform.FindChild("Body").gameObject;
        _connection1 = transform.FindChild("Connection1").gameObject;
        _connection2 = transform.FindChild("Connection2").gameObject;
        _sections = new List<SectionInfo>();
        _currPossibleConnections = new List<TubesTip>();
        _sections.Add(
            new SectionInfo
            {
                length = 1,
                startDir = startDirection,
                endDir = endDirection,
                body = _body,
                startTip = _connection1,
                endTip = _connection2,
                initialPosition = Vector3.zero
            }
        );
        name = "Tube" + uniqueTubeIdIter++;
        _colliders = new List<BoxCollider>();

        MaxInputFlow = 40.0;
    }

    override protected void VirtualStart()
    {
		base.VirtualStart();

        var startSection = _sections[0];
        startSection.startDir = startDirection;
        SetSectionDirection(0, endDirection);
        _globalEndPosition = new Vector3();
        _colliders.Add(GetComponent<BoxCollider>());
        Capacity = 10.0;
    }

    override public void AddInput(Structure connectedStructure)
    {
        base.AddInput(connectedStructure);
        if (inputStructures.Count == 2)
        {
            // Remove all possible connections and tips;
            RemoveAllCurrPossibleConnections();
            for (int dirIndex = 1; dirIndex < 7; dirIndex++)
            {
                var extensionTip = _extensionTips[dirIndex - 1];
                if (extensionTip != null)
                {
                    Destroy(extensionTip.gameObject);
                    _extensionTips[dirIndex - 1] = null;
                }
            }

            // TODO: Set the tube as active, he can start to be updated;
            Opened = true;
        }
    }

    private void RemoveAllCurrPossibleConnections()
    {
        // Unmark the conections not maded and clear the curr conection tips list;
        foreach (var connectionTip in _currPossibleConnections)
        {
            connectionTip.RemovePossibleConnection(this);
        }
        _currPossibleConnections.Clear();
    }

    public void ExtendTube(GridManager.Direction dir)
    {
        RemoveAllCurrPossibleConnections();

        var currSec = _sections[_sections.Count - 1];
        var directionInc = GridManager.DirectionIncrement(dir);

        // resposition the global end position
        _globalEndPosition += directionInc;

        Capacity += 10.0;

        if (endDirection != dir)
        {
            // bend the current section in the new direction;
            SetSectionDirection(_sections.Count - 1, dir);


            var lastCollider = _colliders[_colliders.Count - 1];
            if (lastCollider.size.x > 1 || lastCollider.size.y > 1 || lastCollider.size.z > 1)
            {
                // add a new box collider
                var boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.center = _globalEndPosition;
                _colliders.Add(boxCollider);
            }
            else
            {
                ExtendCollider(lastCollider, directionInc);
            }
        }
        else
        {
            // TODO: remove one of the length of the curr section and add a new curve section at the end;
            // set this new section as the current one;

            // extend the current last collider;
            ExtendCollider(_colliders[_colliders.Count - 1], directionInc);
        }

        //finish the current section and add the new sections;
        CreateNewSection(dir);
        endDirection = dir;

        // change the extension tips position, destroi and create new ones if necessary
        for (int dirIndex = 1; dirIndex < 7; dirIndex++)
        {
            var extensionTip = _extensionTips[dirIndex - 1];
            GridManager.Direction curDirection = (GridManager.Direction)dirIndex;
            GridManager.Direction reverseDirection = GridManager.instance.OppositeDir(dir);
            // if the tip is in the reverse direction  just ignore it or delete the existing one of that direction
            if (reverseDirection == curDirection)
            {
                if (extensionTip != null)
                {
                    // remove the reverse tip
                    DestroyImmediate(extensionTip.gameObject);
                    _extensionTips[dirIndex - 1] = null;
                }
                continue;
            }
            Vector3 directionVector = GridManager.DirectionIncrement(curDirection);
            RaycastHit hitInfo;
            Vector3 relativeTubesEnd = GetTubesEndPosition();
            bool hitSomething = Physics.Raycast(relativeTubesEnd + transform.position, directionVector, out hitInfo, 1);
            if (extensionTip != null && !hitSomething)
            {
                extensionTip.MoveTo(relativeTubesEnd);
            }
            else if (extensionTip == null && !hitSomething)
            {
                // create new extension tip
                var tip = Instantiate(GridManager.instance.TubesTip);
                var tipScript = tip.GetComponent<TubesTip>();
                tipScript.transform.parent = transform;
                tipScript.MoveTo(GetTubesEndPosition());
                tipScript.SetDirection(curDirection);
                tipScript.SetTipType(TubesTip.TubeTipType.Extension);
                tipScript.SetParentStructure(this);
                tipScript.name = name + "Tip" + dirIndex;
                _extensionTips[dirIndex - 1] = tipScript;
            }
            else if (extensionTip != null && hitSomething)
            {
                // remove tip
                DestroyImmediate(extensionTip.gameObject);
                _extensionTips[dirIndex - 1] = null;
            }

            if (hitSomething && hitInfo.collider.gameObject.tag == "TubeTip")
            {
                GameObject hitObject = hitInfo.collider.gameObject;
                TubesTip tubesTip = hitObject.GetComponent<TubesTip>();
                var reverseCurDirection = GridManager.instance.OppositeDir(curDirection);
                tubesTip.AddPossibleConnection(new KeyValuePair<GridManager.Direction, Tube>(reverseCurDirection, this));
                _currPossibleConnections.Add(tubesTip);
            }
        }
    }

    private void ExtendCollider(BoxCollider collider, Vector3 sizeIncrement)
    {
        collider.center += sizeIncrement / 2;
        collider.size += new Vector3(Mathf.Abs(sizeIncrement.x), Mathf.Abs(sizeIncrement.y), Mathf.Abs(sizeIncrement.z));
    }

    public void CreateExtensionTips(TubesTip.TipIOType tipIOType)
    {
        _extensionTips = new List<TubesTip>();
        for (int dirIndex = 1; dirIndex < 7; dirIndex++)
        {
            GridManager.Direction direction = (GridManager.Direction)dirIndex;
            Vector3 directionVector = GridManager.DirectionIncrement(direction);
            RaycastHit hitInfo;
            bool hitSomething = Physics.Raycast(transform.position, directionVector, out hitInfo, 1);
            if (direction != startDirection && !hitSomething)
            {
                var tip = Instantiate(GridManager.instance.TubesTip);
                var tipScript = tip.GetComponent<TubesTip>();
                tipScript.transform.parent = transform;
                tipScript.transform.localPosition = Vector3.zero;
                tipScript.SetDirection(direction);
                tipScript.name = name + "Tip" + dirIndex;
                tipScript.SetTipType(TubesTip.TubeTipType.Extension);
                tipScript.SetParentStructure(this);
				tipScript.tipIOType = tipIOType;
                _extensionTips.Add(tipScript);
            }
            else
            {
                _extensionTips.Add(null);
            }

            if (hitSomething && hitInfo.collider.gameObject.tag == "TubeTip")
            {
                GameObject hitObject = hitInfo.collider.gameObject;
                TubesTip tubesTip = hitObject.GetComponent<TubesTip>();
                GridManager.Direction reverseDirection = GridManager.instance.OppositeDir(direction);
                tubesTip.AddPossibleConnection(new KeyValuePair<GridManager.Direction, Tube>(reverseDirection, this));
                _currPossibleConnections.Add(tubesTip);
            }
        }
    }

    public void CreateNewSection(GridManager.Direction dir)
    {
        var currSec = _sections[_sections.Count - 1];
        var sectionInfo = new SectionInfo();
        sectionInfo.startDir = GridManager.instance.OppositeDir(currSec.endDir);
        sectionInfo.endDir = dir;
        sectionInfo.length = 1;
        sectionInfo.startTip = currSec.endTip;
        sectionInfo.endTip = Instantiate(sectionInfo.startTip);
        sectionInfo.endTip.transform.parent = transform;
        sectionInfo.endTip.name = "Connection" + (_sections.Count + 2);
        sectionInfo.body = Instantiate(GridManager.instance.TubeBodyStraight);
        sectionInfo.body.transform.parent = transform;
        sectionInfo.body.name = "Body";
        sectionInfo.initialPosition = currSec.initialPosition +
            (GridManager.DirectionIncrement(currSec.endDir) * currSec.length);
        _sections.Add(sectionInfo);
        SetSectionDirection(_sections.Count - 1, dir);
    }

    public Vector3 GetTubesEndPosition()
    {
        return _globalEndPosition;
    }

    public void SetSize(int size)
    {
        _body.transform.localScale = new Vector3(1f, 1f, size);
        _body.transform.localPosition = new Vector3(0f, 0f, size / 2);
        var boxDistance = ((size / 2) - 0.5f);
        var directionVector = GridManager.DirectionIncrement(endDirection);
        var sizeVector = directionVector * (size - 1);
        sizeVector = new Vector3(Mathf.Abs(sizeVector.x), Mathf.Abs(sizeVector.y), Mathf.Abs(sizeVector.z));
        _connection2.SetActive(size == 0);
        _size = size;
    }

    public void SetTipDirection(GridManager.Direction to)
    {
        SetSectionDirection(_sections.Count - 1, to);
    }

    public void SetSectionDirection(int sectionId, GridManager.Direction to)
    {
        var currSec = _sections[sectionId];
        var dirVec = new Vector2((int)currSec.startDir, (int)to);
        _reversed = currSec.startDir > to;
        if (_reversed)
        {
            dirVec = new Vector2((int)to, (int)currSec.startDir);
        }
        var dirBodyInfo = BodyDirTable[dirVec];

        if (dirBodyInfo.id > 3 && !currSec.isCurve)
        {
            // swap to curve
            Destroy(currSec.body);
            currSec.body = Instantiate(GridManager.instance.TubeBodyCurve);
            currSec.body.transform.parent = transform;
            currSec.body.name = "Body";
            currSec.isCurve = true;
        }
        else if (dirBodyInfo.id <= 3 && currSec.isCurve)
        {
            // swap to straight
            Destroy(currSec.body);
            currSec.body = Instantiate(GridManager.instance.TubeBodyStraight);
            currSec.body.transform.parent = transform;
            currSec.body.name = "Body";
            currSec.isCurve = false;
        }

        currSec.body.transform.localEulerAngles = dirBodyInfo.rotation;
        currSec.body.transform.localPosition = (GridManager.DirectionIncrement(to) * ((_size - 1f) / 2f)) + currSec.initialPosition;
        currSec.startTip.transform.localEulerAngles = TipDirTable[currSec.startDir].rotation;
        currSec.startTip.transform.localPosition = TipDirTable[currSec.startDir].position + currSec.initialPosition;
        currSec.endTip.transform.localEulerAngles = TipDirTable[to].rotation;
        currSec.endTip.transform.localPosition = TipDirTable[to].position + currSec.initialPosition;

        currSec.endDir = to;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
