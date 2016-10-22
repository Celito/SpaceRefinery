using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoStructure : MonoBehaviour
{
	public List<DemoStructure> inputs = new List<DemoStructure>();
	public List<DemoStructure> outputs = new List<DemoStructure>();

	public GameObject product = null; // This really needs some sort of list, so that items that take multiple inputs can act accordingly
	public DemoStructure destination = null; // TODO: set this more intelligently

	public void Start()
	{
		DemoUpdateManager.instance.Register(this);
	}

	virtual public void Process()
	{

	}
}
