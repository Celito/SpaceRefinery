using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
	public static UpdateManager instance { get { return _instance; } }
	private static UpdateManager _instance;

	public float tickFrequency = 0.5f;

    public float timePassFromLastTick { get { return _timeAccumulator; } }
	
	private List<Structure> _structures = new List<Structure>();
	private List<Structure> _endPoints = new List<Structure>();

	private float _timeAccumulator;

	// attempt to optimize:
	private List<Structure> _cleanPath = new List<Structure>();
	private bool _dirty = false;	

	public void Register(Structure structure)
	{
		_structures.Add(structure);

		UpdateEndPoints();

		_dirty = true;
	}

	public void UpdateEndPoints()
	{
		_endPoints.Clear();

		Dictionary<Structure, bool> searchedDemoStructures = new Dictionary<Structure, bool>();
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

			searchedDemoStructures.Add(current, true);

			if(current.OutputCount > 0)
			{
				for(int i = 0; i < current.OutputCount; i++)
				{
                    Structure potential = current.GetOutput(i);

					if (searchedDemoStructures.ContainsKey(potential) == false)
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

		if(_timeAccumulator >= tickFrequency)
		{
			_timeAccumulator -= tickFrequency;

			if(_dirty == false)
			{
				// optimized "clean" path
				for (int i = 0; i < _cleanPath.Count; ++i)
					_cleanPath[i].Process();
			}
			else
			{
				// dirty path
				_cleanPath.Clear();
				
				Dictionary<Structure, bool> visitedDemoStructures = new Dictionary<Structure, bool>();
				Queue<Structure> pendingDemoStructures = new Queue<Structure>();

				foreach (Structure endPoint in _endPoints)
				{
					pendingDemoStructures.Enqueue(endPoint);

					visitedDemoStructures.Add(endPoint, true);
				}

				while (pendingDemoStructures.Count > 0)
				{
					Structure structure = pendingDemoStructures.Dequeue();

					structure.Process();

					// add this to the clean path
					_cleanPath.Add(structure);

					for (int i = 0; i < structure.InputCount; i++)
					{
						Structure input = structure.GetInput(i);

						if (visitedDemoStructures.ContainsKey(input) == false)
						{
							pendingDemoStructures.Enqueue(input);

							visitedDemoStructures.Add(input, true);
						}
					}
				}

				_dirty = false;
			}
		}
	}
}
