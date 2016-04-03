using UnityEngine;
using System;
using System.Collections.Generic;

public class Output : MonoBehaviour
{
    public GameObject tipRef;

    private List<TubesTip> _initialPipeTips;
    //private PointOfInterest _poi;

    // Use this for initialization
    void Start ()
    {
        //_poi = GetComponent<PointOfInterest>();
        //_poi.OnSelected += onSelected;
        //_poi.OnDeselected += onDeselected;
        _initialPipeTips = new List<TubesTip>();
        for(int i = 0; i < transform.childCount; i++)
        {
            TubesTip childrenTip = transform.GetChild(i).GetComponent<TubesTip>();
            if (childrenTip)
            {
                _initialPipeTips.Add(childrenTip);
                //childrenTip.gameObject.SetActive(false);
            }
        }

	}

    void onSelected()
    {
        foreach(TubesTip tip in _initialPipeTips)
        {
            //tip.gameObject.SetActive(true);
        }
    }

    void onDeselected()
    {
        foreach (TubesTip tip in _initialPipeTips)
        {
            //tip.gameObject.SetActive(false);
        }
    }
}
