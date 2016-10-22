using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterStructure : Structure
{
	// doesn't check at all if outputs exist

	int counter = 0;

	// left
	private int _side = -1;

	override public void Process()
	{
		if(_products.Count == 1)
		{
			counter += 1;

			if(counter == 1)
			{
				// move it to the middle
				_products[0].transform.position = transform.position + Vector3.up;
			}
			else if(counter == 2)
			{
				// which way are we going?
				if (_side == -1)
				{
					_products[0].transform.position = transform.position + Vector3.up + Vector3.left;
				}
				else
				{
					_products[0].transform.position = transform.position + Vector3.up + Vector3.right;
				}
			}
            else if(counter >= 3)
            {
                if((_side == -1 && _outputs[0].Receive(this, _products[0])) || _outputs[1].Receive(this, _products[0]))
                {
                    _products.RemoveAt(0);
                    _side *= -1;
                    counter = 0;
                }
            }
		}
	}

    public override bool Receive(Structure input, Product product)
    {
        if (_products.Count == 0)
        {
            _products.Add(product);
            product.transform.position = transform.position + Vector3.up - Vector3.forward;
            return true;
        }
        else
        {
            return false;
        }
    }
}
