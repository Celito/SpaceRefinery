using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorStructure : DemoStructure
{
	int counter = 0;
	int frequency = 1;

	override public void Process()
	{
		counter += 1;

		// we're 1 way only
		if (outputs.Count > 0)
			destination = outputs[0];
		else
			destination = null;

		if (counter == frequency)
		{
			counter = 0;

			if(inputs.Count != 0)
			{
				if(product == null && inputs[0].product != null && inputs[0].destination == this)
				{
					product = inputs[0].product;
					product.transform.position = transform.position + Vector3.up;

					inputs[0].product = null;
				}
			}
		}
	}
}
