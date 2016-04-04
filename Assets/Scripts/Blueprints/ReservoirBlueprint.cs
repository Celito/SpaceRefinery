using UnityEngine;
using System.Collections;

public class ReservoirBlueprint : Blueprint
{

    public GameObject ReservoirRef;

    private BoxCollider _boxCollider;
    private int _collingWith = 0;

    // Use this for initialization
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        _collingWith++;        
        if(_collingWith == 1)
        {
            SetEnableToBuild(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        _collingWith--;
        if(_collingWith == 0)
        {
            SetEnableToBuild(true);
        }
    }

    void OnMouseDown()
    {
        if(_collingWith == 0)
        {
            var reservoir = Instantiate(ReservoirRef);
            reservoir.transform.position = transform.position;
        }
    }

    override public void MoveBlueprintTo(Vector3 pos)
    {
        base.MoveBlueprintTo(pos);
    }
}
