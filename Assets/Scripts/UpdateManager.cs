using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpdateManager : MonoBehaviour
{
	public static UpdateManager instance { get { return _instance; } }
	private static UpdateManager _instance;

	public float frequency = 1.0f;
	
	private List<OldStructure> _structures = new List<OldStructure>();
	private List<OldStructure> _endPoints = new List<OldStructure>();

	private float _timeAccumulator;

	public void Register(OldStructure structure)
	{
		_structures.Add(structure);

		UpdateEndPoints();
	}

	public void UpdateEndPoints()
	{
		_endPoints.Clear();

		List<OldStructure> searchedStructures = new List<OldStructure>();		
		Queue<OldStructure> pendingStructures = new Queue<OldStructure>();

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
			OldStructure current = pendingStructures.Dequeue();

			searchedStructures.Add(current);

			if(current.outputStructures.Count > 0)
			{
				foreach (OldStructure potential in current.outputStructures)
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
