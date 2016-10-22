using UnityEngine;
using System.Collections.Generic;

public class OldStructure : MonoBehaviour {

    public double MaxInputFlow;
    public double Capacity;
    public double CurrLoad;
    public double CurrPresure;
    public bool Opened;

    public readonly List<OldStructure> inputStructures = new List<OldStructure>();
	public readonly List<OldStructure> outputStructures = new List<OldStructure>();
    
    void Awake()
    {
        VirtualAwake();
    }

    virtual protected void VirtualAwake()
    {
		
    }

    void Start()
    {
        VirtualStart();
        //FlowManager.instance.AddNewStructure(this);
    }

    virtual protected void VirtualStart()
	{
		OldUpdateManager.instance.Register(this);
	}

    virtual public void AddInput(OldStructure connectedStructure)
    {
        if (!inputStructures.Contains(connectedStructure))
        {
            inputStructures.Add(connectedStructure);
            connectedStructure.AddOutput(this);
        }
    }

	virtual public void AddOutput(OldStructure connectedStructure)
	{
		if (!outputStructures.Contains(connectedStructure))
		{
			outputStructures.Add(connectedStructure);
			connectedStructure.AddInput(this);
		}
	}


    virtual public void ProcessFlow(double deltaTime)
    {
        foreach (var connection in inputStructures)
        {
            if (connection.CurrPresure - CurrPresure < -0.005 && connection.Opened)
            {
                var outputFlow = MaxInputFlow * deltaTime * CurrPresure;
                if (outputFlow > CurrLoad) outputFlow = CurrLoad;
                var totalFlowSent = connection.ReceiveFlow(outputFlow, deltaTime, this);
                CurrLoad -= totalFlowSent;
                CurrPresure = CurrLoad / Capacity;
            }
        }
    }

    virtual public double ReceiveFlow(double flow, double deltaTime, OldStructure sender)
    {
        CurrLoad += flow;
        CurrPresure = CurrLoad / Capacity;
        return flow;
    }
}
