using UnityEngine;
using System.Collections.Generic;

public class Structure : MonoBehaviour {

    public double MaxInputFlow;
    public double Capacity;
    public double CurrLoad;
    public double CurrPresure;
    public bool Opened;

    protected List<Structure> _connectedStructures;
    
    void Awake()
    {
        VirtualAwake();
    }

    virtual protected void VirtualAwake()
    {
        if (_connectedStructures == null)
        {
            _connectedStructures = new List<Structure>();
        }
    }

    void Start()
    {
        VirtualStart();
        FlowManager.instance.AddNewStructure(this);
    }

    virtual protected void VirtualStart(){}

    virtual public void ConnectTo(Structure connectedStructure)
    {
        if (_connectedStructures == null)
        {
            _connectedStructures = new List<Structure>();
        }
        if (!_connectedStructures.Contains(connectedStructure))
        {
            _connectedStructures.Add(connectedStructure);
            connectedStructure.ConnectTo(this);
        }
    }

    virtual public void ProcessFlow(double deltaTime)
    {
        foreach (var connection in _connectedStructures)
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

    virtual public double ReceiveFlow(double flow, double deltaTime, Structure sender)
    {
        CurrLoad += flow;
        CurrPresure = CurrLoad / Capacity;
        return flow;
    }
}
