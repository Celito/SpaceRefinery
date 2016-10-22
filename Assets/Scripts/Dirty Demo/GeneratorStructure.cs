using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorStructure : DemoStructure
{
	public GameObject productPrefab;

	int counter = 0;
	int frequency = 2;

	override public void Process()
	{
		Debug.Log("Generator Processing");

		counter += 1;

		if (counter == frequency)
		{
			counter = 0;

			if (product == null)
			{
				product = Instantiate<GameObject>(productPrefab);
				product.transform.position = transform.position + Vector3.up;

				destination = outputs[0];
			}
		}	
	}
}
