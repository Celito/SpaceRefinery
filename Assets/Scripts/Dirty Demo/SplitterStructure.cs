using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterStructure : DemoStructure
{
	// doesn't check at all if outputs exist

	int counter = 0;

	// left
	int side = -1;

	override public void Process()
	{
		if(product != null)
		{
			counter += 1;

			if(counter == 1)
			{
				// move it to the middle
				product.transform.position = transform.position + Vector3.up;
			}
			else if(counter == 2)
			{
				// which way are we going?
				if (side == -1)
				{
					product.transform.position = transform.position + Vector3.up + Vector3.left;

					destination = outputs[0];

					side *= -1;
				}
				else
				{
					product.transform.position = transform.position + Vector3.up + Vector3.right;

					destination = outputs[1];

					side *= -1;
				}
			}
		}
		else
		{
			counter = 0;
			destination = null;

			// no product, try to get one
			if(inputs.Count != 0)
			{
				if(inputs[0].product != null && inputs[0].destination == this)
				{
					product = inputs[0].product;
					product.transform.position = transform.position + Vector3.up - Vector3.forward;

					inputs[0].product = null;
				}
			}
		}
	}
}
