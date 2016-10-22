using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoUpdateManager : MonoBehaviour
{
	public static DemoUpdateManager instance { get { return _instance; } }
	private static DemoUpdateManager _instance;

	public float frequency = 0.5f;
	
	private List<DemoStructure> _structures = new List<DemoStructure>();
	private List<DemoStructure> _endPoints = new List<DemoStructure>();

	private float _timeAccumulator;

	public void Register(DemoStructure structure)
	{
		_structures.Add(structure);

		UpdateEndPoints();
	}

	public void UpdateEndPoints()
	{
		_endPoints.Clear();

		List<DemoStructure> searchedDemoStructures = new List<DemoStructure>();		
		Queue<DemoStructure> pendingDemoStructures = new Queue<DemoStructure>();

		for(int i = 0; i < _structures.Count; ++i)
		{			
			if(_structures[i].inputs.Count == 0)
			{
				pendingDemoStructures.Enqueue(_structures[i]);
			}
		}

		// Debug.Log("Pending DemoStructures: " + pendingDemoStructures.Count);

		while(pendingDemoStructures.Count > 0)
		{
			DemoStructure current = pendingDemoStructures.Dequeue();

			searchedDemoStructures.Add(current);

			if(current.outputs.Count > 0)
			{
				foreach (DemoStructure potential in current.outputs)
				{
					if(searchedDemoStructures.IndexOf(potential) == -1)
						pendingDemoStructures.Enqueue(potential);
				}
			}
			else
			{
				_endPoints.Add(current);
			}
		}

		// Debug.Log("Found " + _endPoints.Count + " end points");
	}

	private void Awake()
	{
		_instance = this;
	}

	// TODO: We can optimize this very easily by creating a List<> that contains the "correct path" after we build it from the queue
	// and use that path every single time UNLESS a module is added or deleted, then we can mark it as "dirty" and
	// rebuild it from the queue version.

	private void Update()
	{
		_timeAccumulator += Time.deltaTime;

		if(_timeAccumulator >= frequency)
		{
			_timeAccumulator -= frequency;

			// do update stuff
			List<DemoStructure> visitedDemoStructures = new List<DemoStructure>(); // should be a dictionary for O(1) search time
			Queue<DemoStructure> pendingDemoStructures = new Queue<DemoStructure>();

			foreach (DemoStructure endPoint in _endPoints)
			{
				pendingDemoStructures.Enqueue(endPoint);

				visitedDemoStructures.Add(endPoint);
			}

			while(pendingDemoStructures.Count > 0)
			{
				DemoStructure structure = pendingDemoStructures.Dequeue();

				structure.Process();

				foreach (DemoStructure input in structure.inputs)
				{
					if (visitedDemoStructures.IndexOf(input) == -1)
					{
						pendingDemoStructures.Enqueue(input);

						visitedDemoStructures.Add(input);
					}
				}
			}
		}
	}
}
