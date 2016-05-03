using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tube : MonoBehaviour
{

    public GridManager.Direction startDirection = GridManager.Direction.FRONT;
    public GridManager.Direction endDirection = GridManager.Direction.BACK;

    private GameObject _body;
    private GameObject _connection1;
    private GameObject _connection2;
    private BoxCollider _boxCollider;

    private List<TubesTip> _extensionTips;

    private Vector2 _currConexion = new Vector2(3, 4); // the prefeb is positioned at the direction 3 by default;
    private bool _reversed;
    private int _size = 1;

    private List<SectionInfo> _sections;
    private Vector3 _globalEndPosition;

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
    void Awake ()
    {
        _body = transform.FindChild("Body").gameObject;
        _connection1 = transform.FindChild("Connection1").gameObject;
        _connection2 = transform.FindChild("Connection2").gameObject;
        _sections = new List<SectionInfo>();
        _sections.Add(
            new SectionInfo {
                length = 1, startDir = startDirection, endDir = endDirection,
                body = _body, startTip = _connection1, endTip = _connection2, initialPosition = Vector3.zero }
        );
        _boxCollider = GetComponent<BoxCollider>();
    }

    void Start ()
    {   
        var startSection = _sections[0];
        startSection.startDir = startDirection;
        SetSectionDirection(0, endDirection);
        _globalEndPosition = new Vector3();
    }

    public void ExtendTube(GridManager.Direction dir)
    {
        var currSec = _sections[_sections.Count - 1];
        if (endDirection != dir)
        {
            // bend the current section in the new direction;
            SetSectionDirection(_sections.Count - 1, dir);
        }
        else
        {
            // remove one of the length of the curr section and add a new curve section at the end;
            // set this new section as the current one;
        }

        //finish the current section and add the new sections;
        CreateNewSection(dir);
        endDirection = dir;

        // resposition the global end position
        _globalEndPosition += GridManager.DirectionIncrement(dir);

        //change teh extension tips position, destroi and create new ones if necessary
        for(int dirIndex = 1; dirIndex < 7; dirIndex++)
        {
            var extensionTip = _extensionTips[dirIndex - 1];
            GridManager.Direction curDirection = (GridManager.Direction)dirIndex;
            GridManager.Direction reverseDirection = GridManager.instance.OppositeDir(dir);
            if(reverseDirection == curDirection)
            {
                if(extensionTip != null)
                {
                    // remove the reverse tip
                    DestroyImmediate(extensionTip.gameObject);
                    _extensionTips[dirIndex - 1] = null;
                }
                continue;
            }
            Vector3 directionVector = GridManager.DirectionIncrement(curDirection);
            RaycastHit hitInfo;
            bool hitSomething = Physics.Raycast(GetTubesEndPosition(), directionVector, out hitInfo, 1);
            if (extensionTip != null && !hitSomething)
            {
                extensionTip.MoveTo(GetTubesEndPosition());
            }
            else if(extensionTip == null && !hitSomething)
            {
                // create new extension tip
                var tip = Instantiate(GridManager.instance.TubesTip);
                var tipScript = tip.GetComponent<TubesTip>();
                tipScript.transform.parent = transform;
                tipScript.MoveTo(GetTubesEndPosition());
                tipScript.SetDirection(curDirection);
                //tipScript.OnTubeCreated += ExtensionCreated;
                tipScript.SetType(TubesTip.TubeTipType.Extension);
                tipScript.SetParentTube(this);
                _extensionTips[dirIndex - 1] = tipScript;
            } 
            else if (extensionTip != null && hitSomething)
            {
                // remove tip
                DestroyImmediate(extensionTip.gameObject);
                _extensionTips[dirIndex - 1] = null;
            }
        }
    }

    public void CreateExtensionTips()
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
                //tipScript.OnTubeCreated += ExtensionCreated;
                tipScript.SetType(TubesTip.TubeTipType.Extension);
                tipScript.SetParentTube(this);
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
                tubesTip.SetType(TubesTip.TubeTipType.Conection);
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
        _boxCollider.size = (Vector3.one) + sizeVector;
        _boxCollider.center = directionVector * boxDistance;
        _connection2.SetActive(size == 0);
        _size = size;
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
    void Update ()
    {
	
	}
}
