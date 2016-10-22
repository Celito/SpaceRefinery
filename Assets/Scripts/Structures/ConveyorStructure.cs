using UnityEngine;

public class ConveyorStructure : Structure
{

	override public void Process()
    {
        if (_products.Count > 0 && _outputs.Count == 1)
        {
            // TODO: Needs to be changed when we have conveyrs bigger then 1 slot;
            if(_outputs[0].Receive(this, _products[0]))
            {
                _products.RemoveAt(0);
            }
        }
	}

    public override bool Receive(Structure input, Product product)
    {
        if(_products.Count == 0)
        {
            _products.Add(product);
            product.transform.position = transform.position + Vector3.up;
            return true;
        }
        else
        {
            return false;
        }
    }
}
