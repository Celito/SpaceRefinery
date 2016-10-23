using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRandomBuilder : MonoBehaviour // not really random at all :P
{
	public GameObject generatorPrefab;
	public GameObject conveyorPrefab;
	public GameObject splitterPrefab;

    public int numberOfTestReplicas = 0;

    public int targetNumberOfReplicas = 1;

	public void Start()
	{
        StartCoroutine(CreateTestElements());
		
	}

    IEnumerator CreateTestElements()
    {
        while(numberOfTestReplicas < targetNumberOfReplicas)
        {
            numberOfTestReplicas++;
            GameObject startPoint = Instantiate<GameObject>(generatorPrefab);
            Move(startPoint, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));

            GameObject lastStructure = startPoint;

            for (int i = 0; i < 5; ++i)
            {
                GameObject conveyor = Instantiate<GameObject>(conveyorPrefab);
                conveyor.name = "Conveyor " + i;

                Move(conveyor, new Vector3(0f, 0f, 1.0f + (i * 1.0f)), Quaternion.Euler(0f, 0f, 0f));

                Connect(lastStructure, conveyor);

                lastStructure = conveyor;
            }

            GameObject splitter = Instantiate<GameObject>(splitterPrefab);
            Move(splitter, lastStructure.transform.position + new Vector3(0.0f, 0.0f, 2.0f), Quaternion.Euler(0f, 0f, 0f));

            Connect(lastStructure, splitter);

            // build left
            lastStructure = splitter;

            for (int i = 0; i < 5; ++i)
            {
                GameObject conveyor = Instantiate<GameObject>(conveyorPrefab);
                conveyor.name = "Left Conveyor " + i;

                Move(conveyor, new Vector3(-2.0f - (i * 1.0f), 0f, splitter.transform.position.z), Quaternion.Euler(0f, 0f, 0f));

                Connect(lastStructure, conveyor);

                lastStructure = conveyor;
            }

            // build right
            lastStructure = splitter;

            for (int i = 0; i < 5; ++i)
            {
                GameObject conveyor = Instantiate<GameObject>(conveyorPrefab);
                conveyor.name = "Right Conveyor " + i;

                Move(conveyor, new Vector3(2.0f + (i * 1.0f), 0f, splitter.transform.position.z), Quaternion.Euler(0f, 0f, 0f));

                Connect(lastStructure, conveyor);

                lastStructure = conveyor;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

	protected void Move(GameObject structure, Vector3 position, Quaternion rotation)
	{
		structure.transform.position = position;
		structure.transform.rotation = rotation;
	}

	protected void Connect(GameObject input, GameObject output)
	{
		Structure inputStructure = input.GetComponent<Structure>();
		Structure outputStructure = output.GetComponent<Structure>();

		inputStructure.CreateOutputConnection(outputStructure);
		outputStructure.CreateInputConnection(inputStructure);
	}
}
