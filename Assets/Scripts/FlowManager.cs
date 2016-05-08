using UnityEngine;
using System.Collections.Generic;

public class FlowManager : MonoBehaviour
{
    public static FlowManager instance;

    private List<Structure> _structure;

    void Awake ()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        _structure = new List<Structure>();
    }
    
    void Start ()
    {

    }

	void FixedUpdate ()
    {
	    foreach(var structure in _structure)
        {
            if(structure.Opened)
            {
                structure.ProcessFlow(Time.fixedDeltaTime);
            }
        }
	}

    public void AddNewStructure(Structure structure)
    {
        _structure.Add(structure);
    }
}
