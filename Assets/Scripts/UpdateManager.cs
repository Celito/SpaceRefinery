using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpdateManager : MonoBehaviour
{
	public static UpdateManager instance { get { return _instance; } }
	private static UpdateManager _instance;

	public float frequency = 1.0f;
	
	private List<Structure> _structures = new List<Structure>();
	private List<Structure> _endPoints = new List<Structure>();

	private float _timeAccumulator;

	public void Register(Structure structure)
	{
		_structures.Add(structure);

		UpdateEndPoints();
	}

	public void UpdateEndPoints()
	{
		_endPoints.Clear();

		List<Structure> searchedStructures = new List<Structure>();		
		Queue<Structure> pendingStructures = new Queue<Structure>();

		for(int i = 0; i < _structures.Count; ++i)
		{			
			if(_structures[i].inputStructures.Count == 0)
			{
				pendingStructures.Enqueue(_structures[i]);
			}
		}

		Debug.Log("Pending Structures: " + pendingStructures.Count);

		while(pendingStructures.Count > 0)
		{
			Structure current = pendingStructures.Dequeue();

			searchedStructures.Add(current);

			if(current.outputStructures.Count > 0)
			{
				foreach (Structure potential in current.outputStructures)
				{
					if(searchedStructures.IndexOf(potential) == -1)
						pendingStructures.Enqueue(potential);
				}
			}
			else
			{
				_endPoints.Add(current);
			}
		}

		Debug.Log("Found " + _endPoints.Count + " end points");
	}

	private void Awake()
	{
		_instance = this;
	}

	private void Update()
	{
		_timeAccumulator += Time.deltaTime;

		if(_timeAccumulator >= frequency)
		{
			_timeAccumulator -= frequency;

			// do update stuff



		}
	}
}
