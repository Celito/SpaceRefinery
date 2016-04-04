using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Tube : MonoBehaviour
{

    public GridManager.Direction startDirection = GridManager.Direction.FRONT;
    public GridManager.Direction endDirection = GridManager.Direction.BACK;

    private GameObject _body;
    private GameObject _tip1;
    private GameObject _tip2;
    private BoxCollider _boxCollider;

    private List<TubesTip> _extensionTips;

    private Vector2 _currConexion = new Vector2(3, 4); // the prefeb is positioned at the direction 3 by default;
    private bool _reversed;
    private int _size = 1;

    struct DirectionInfo
    {
        public int id;
        public Vector3 rotation;
        public Vector3 position;
    }

    private static Dictionary<Vector2, DirectionInfo> _bodyDirTable = new Dictionary<Vector2, DirectionInfo>()
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

    private static Dictionary<GridManager.Direction, DirectionInfo> _tipDirTable = 
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
        _tip1 = transform.FindChild("Tip1").gameObject;
        _tip2 = transform.FindChild("Tip2").gameObject;
        _boxCollider = GetComponent<BoxCollider>();
    }

    void Start ()
    {
        SetDirection(startDirection, endDirection);
        
    }

    public void CreateExtensionTips()
    {
        _extensionTips = new List<TubesTip>();
        for (int dirIndex = 1; dirIndex < 7; dirIndex++)
        {
            GridManager.Direction direction = (GridManager.Direction)dirIndex;
            Vector3 directionVector = GridManager.DirectionIncrement(direction);
            RaycastHit hitInfo;
            if (direction != startDirection && !Physics.Raycast(transform.position, directionVector, out hitInfo, 1))
            {
                var tip = Instantiate(GridManager.instance.TubesTip);
                var tipScript = tip.GetComponent<TubesTip>();
                tipScript.transform.parent = transform;
                tipScript.transform.localPosition = Vector3.zero;
                tipScript.SetDirection(direction);
                tipScript.OnTubeCreated += ExtensionCreated;
                _extensionTips.Add(tipScript);
            }
        }
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
        _tip2.SetActive(size == 0);
        _size = size;
    }

    void ExtensionCreated(Tube tube)
    {
        _extensionTips.ForEach((tip) => { tip.gameObject.SetActive(false); });
        SetDirection(this.startDirection, tube.endDirection);
    }

    public void SetDirection(GridManager.Direction from, GridManager.Direction to)
    {
        var currDirBodyInfo = _bodyDirTable[_currConexion];
        var dirVec = new Vector2((int)from, (int)to);
        _reversed = from > to;
        if (from > to)
        {
            dirVec = new Vector2((int)to, (int)from);
        }
        var dirBodyInfo = _bodyDirTable[dirVec];

        if (dirBodyInfo.id > 3 && currDirBodyInfo.id <= 3)
        {
            // swap to curve
            Destroy(_body);
            _body = Instantiate(GridManager.instance.TubeBodyCurve);
            _body.transform.parent = transform;
            _body.name = "Body";
        }
        else if(dirBodyInfo.id <= 3 && currDirBodyInfo.id > 3)
        {
            // swap to straight
            Destroy(_body);
            _body = Instantiate(GridManager.instance.TubeBodyStraight);
            _body.transform.parent = transform;
            _body.name = "Body";
        }

        _body.transform.localEulerAngles = dirBodyInfo.rotation;
        _body.transform.localPosition = GridManager.DirectionIncrement(to) * ((_size - 1f) / 2f);
        _tip1.transform.localEulerAngles = _tipDirTable[from].rotation;
        _tip1.transform.localPosition = _tipDirTable[from].position;
        _tip2.transform.localEulerAngles = _tipDirTable[to].rotation;
        _tip2.transform.localPosition = _tipDirTable[to].position;

        startDirection = from;
        endDirection = to;
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
