using UnityEngine;
using System;
using System.Collections.Generic;

public class Output : Structure
{
    public GameObject tipRef;

    private List<TubesTip> _initialPipeTips;

    // Use this for initialization
    override protected void VirtualStart ()
    {
        _initialPipeTips = new List<TubesTip>();
        for(int i = 0; i < transform.childCount; i++)
        {
            TubesTip childrenTip = transform.GetChild(i).GetComponent<TubesTip>();
            if (childrenTip)
            {
                _initialPipeTips.Add(childrenTip);
            }
        }
        Opened = true;
        CurrPresure = 1;
	}

    override public void ConnectTo(Structure connectedStructure)
    {
        base.ConnectTo(connectedStructure);
    }

    public override void ProcessFlow(double deltaTime)
    {
        //TODO: remove this hard coded value
        var outputFlow = deltaTime * 10.0;
        foreach(var connection in _connectedStructures)
        {
            if(connection.CurrPresure < CurrPresure && connection.Opened)
            {
                connection.ReceiveFlow(outputFlow, deltaTime, this);
            }
        }
    }


    public override double ReceiveFlow(double flow, double deltaTime, Structure sender)
    {
        // can't receive anything
        return 0.0;
    }
}
