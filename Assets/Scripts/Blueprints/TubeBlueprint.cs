using UnityEngine;
using System.Collections;

public class TubeBlueprint : Blueprint
{
    private GameObject _body;
    private GameObject _tip1;
    private GameObject _tip2;
    private TubesTip _currTip;
    private bool _isCurve = false;

    void Awake()
    {
        _body = transform.FindChild("Body").gameObject;
        _tip1 = transform.FindChild("Tip1").gameObject;
        _tip2 = transform.FindChild("Tip2").gameObject;
    }

    // Use this for initialization
    void Start ()
    {
	}
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "TubeTip")
        {
            _currTip = other.gameObject.GetComponent<TubesTip>();
            TubesTip tipScript = _currTip.GetComponent<TubesTip>();
            GridManager.Direction reverse = GridManager.instance.OppositeDir(tipScript.direction);
            if (tipScript.GetTipType() != TubesTip.TubeTipType.Connection)
            {
                int dirValue = (int)tipScript.direction;
                if (dirValue > 3) dirValue = 7 - dirValue;
                SetBlueprintDirection(tipScript.direction, reverse);
                _tip2.SetActive(false);
                SetEnableToBuild(true);
            }
            else
            {
                //TODO: display the current connection
                var connectioDir = tipScript.GetPossibleConnectionDirection();
                SetBlueprintDirection(connectioDir, reverse);
                _tip1.SetActive(false);
                _tip2.SetActive(false);
                SetEnableToBuild(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "TubeTip")
        {
            _currTip = null;
            _tip1.SetActive(true);
            _tip2.SetActive(true);
            SetEnableToBuild(false);
        }
    }

    void OnMouseDown()
    {
        if (_currTip && _currTip.gameObject.activeSelf)
        {
            //TODO: set the connection index if it is a connection tube
            _currTip.CreateTube(0);
            SetEnableToBuild(false);
        }
    }

    private void SetBlueprintDirection(GridManager.Direction from, GridManager.Direction to)
    {
        var dirVec = from < to ? new Vector2((int)from, (int)to) : new Vector2((int)to, (int)from);
        var dirBodyInfo = Tube.BodyDirTable[dirVec];

        if (dirBodyInfo.id > 3 && !_isCurve)
        {
            // swap to curve
            var curMaterial = _body.GetComponent<Renderer>().material;
            Destroy(_body);
            _body = Instantiate(GridManager.instance.TubeBodyCurve);
            _body.transform.parent = transform;
            _body.name = "Body";
            _body.GetComponent<Renderer>().material = curMaterial;
            _isCurve = true;
        }
        else if (dirBodyInfo.id <= 3 && _isCurve)
        {
            // swap to straight
            var curMaterial = _body.GetComponent<Renderer>().material;
            Destroy(_body);
            _body = Instantiate(GridManager.instance.TubeBodyStraight);
            _body.transform.parent = transform;
            _body.name = "Body";
            _body.GetComponent<Renderer>().material = curMaterial;
            _isCurve = false;
        }

        _body.transform.localEulerAngles = dirBodyInfo.rotation;
        _body.transform.localPosition = Vector3.zero;
        _tip1.transform.localEulerAngles = Tube.TipDirTable[from].rotation;
        _tip1.transform.localPosition = Tube.TipDirTable[from].position;
        _tip2.transform.localEulerAngles = Tube.TipDirTable[to].rotation;
        _tip2.transform.localPosition = Tube.TipDirTable[to].position;
    }
}
