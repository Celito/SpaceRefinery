using UnityEngine;
using System.Collections;

public class TubeBlueprint : Blueprint
{
    private GameObject _body;
    private GameObject _tip1;
    private GameObject _tip2;
    private GameObject _currTip;

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
            _currTip = other.gameObject;
            TubesTip tipScript = _currTip.GetComponent<TubesTip>();
            if(tipScript.GetTipType() != TubesTip.TubeTipType.Conection)
            {
                GridManager.Direction reverse = GridManager.instance.OppositeDir(tipScript.direction);
                int dirValue = (int)tipScript.direction;
                if (dirValue > 3) dirValue = 7 - dirValue;
                Vector2 desiredDirection = new Vector2(dirValue, 7 - dirValue);
                _body.transform.localEulerAngles = Tube.BodyDirTable[desiredDirection].rotation;
                _tip1.transform.localEulerAngles = Tube.TipDirTable[tipScript.direction].rotation;
                _tip1.transform.localPosition = Tube.TipDirTable[tipScript.direction].position;
                _tip2.transform.localEulerAngles = Tube.TipDirTable[reverse].rotation;
                _tip2.transform.localPosition = Tube.TipDirTable[reverse].position;
                _tip2.SetActive(false);
                SetEnableToBuild(true);
            }
            else
            {
                //TODO
                if (_isEnableToBuild)
                {
                    _currTip = null;
                    _tip2.SetActive(true);
                    SetEnableToBuild(false);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "TubeTip")
        {
            _currTip = null;
            _tip2.SetActive(true);
            SetEnableToBuild(false);
        }
    }

    void OnMouseDown()
    {
        if (_currTip && _currTip.activeSelf)
        {
            _currTip.SendMessage("CreateTube");
        }
    }
}
