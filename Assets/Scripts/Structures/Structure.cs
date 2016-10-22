using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{

    protected List<Structure> _inputs = new List<Structure>();
    protected List<Structure> _outputs = new List<Structure>();
    protected List<Product> _products = new List<Product>();

    public int InputCount { get { return _inputs.Count; } }
    public int OutputCount { get { return _outputs.Count; } }

    public void Start()
	{
		DemoUpdateManager.instance.Register(this);
	}

	virtual public void Process()
	{
        // To be defined by its children classes
	}
    
    virtual public bool Receive(Structure input, Product product)
    {
        // To be defined by its children classes
        return false;
    }

    public Structure GetOutput(int i)
    {
        return _outputs[i];
    }

    public Structure GetInput(int i)
    {
        return _inputs[i];
    }

    internal void CreateInputConnection(Structure otherStructure)
    {
        _inputs.Add(otherStructure);
    }

    internal void CreateOutputConnection(Structure otherStructure)
    {
        _outputs.Add(otherStructure);
    }
}
