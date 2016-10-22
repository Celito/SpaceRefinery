using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoUpdateManager : MonoBehaviour
{
	public static DemoUpdateManager instance { get { return _instance; } }
	private static DemoUpdateManager _instance;

	public float frequency = 0.5f;
	
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

		List<Structure> searchedDemoStructures = new List<Structure>();		
		Queue<Structure> pendingDemoStructures = new Queue<Structure>();

		for(int i = 0; i < _structures.Count; ++i)
		{
			if(_structures[i].InputCount == 0)
			{
				pendingDemoStructures.Enqueue(_structures[i]);
			}
		}

		// Debug.Log("Pending DemoStructures: " + pendingDemoStructures.Count);

		while(pendingDemoStructures.Count > 0)
		{
			Structure current = pendingDemoStructures.Dequeue();

			searchedDemoStructures.Add(current);

			if(current.OutputCount > 0)
			{
				for(int i = 0; i < current.OutputCount; i++)
				{
                    Structure potential = current.GetOutput(i);
					if(searchedDemoStructures.IndexOf(potential) == -1)
						pendingDemoStructures.Enqueue(potential);
				}
			}
			else
			{
				_endPoints.Add(current);
			}
		}
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
			List<Structure> visitedDemoStructures = new List<Structure>(); // should be a dictionary for O(1) search time
			Queue<Structure> pendingDemoStructures = new Queue<Structure>();

			foreach (Structure endPoint in _endPoints)
			{
				pendingDemoStructures.Enqueue(endPoint);

				visitedDemoStructures.Add(endPoint);
			}

			while(pendingDemoStructures.Count > 0)
			{
				Structure structure = pendingDemoStructures.Dequeue();

				structure.Process();

				for(int i = 0; i < structure.InputCount; i++)
				{
                    Structure input = structure.GetInput(i);
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
