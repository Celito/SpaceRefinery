using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorStructure : Structure
{
	public GameObject productPrefab;

	int counter = 0;
	int frequency = 2;

	override public void Process()
	{

        if(_products.Count != 0)
        {
            if (_outputs[0].Receive(this, _products[0]))
            {
                _products.RemoveAt(0);
            }
        }
        else
        {
            counter += 1;

            if (counter == frequency)
            {
                counter = 0;

                if (_products.Count == 0)
                {
                    GameObject newProductGO = Instantiate<GameObject>(productPrefab);
                    Product newProduct = newProductGO.GetComponent<Product>();
                    _products.Add(newProduct);

                    newProductGO.transform.position = transform.position + Vector3.up;
                }
            }
        }
	}
}
