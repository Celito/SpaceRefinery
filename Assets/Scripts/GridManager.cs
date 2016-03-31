using UnityEngine;
using System.Collections;

public class GridManager : MonoBehaviour
{
    public enum Direction { BOTTOM = 1, LEFT, BACK, FRONT, RIGHT, TOP };

    public static GridManager instance = null;

    public float cubesSize = 1.0f;

    public GameObject TubesTip;
    public GameObject ProjectionCube;
    public GameObject TubeBodyStraight;
    public GameObject TubeBodyCurve;

    public GameObject MovementPlane;
    public MeshCollider MovementPlaneMesh;

    public GameObject Tube4;

    public GameObject Tube4Construction;

    public GameObject SelectedConstructionPart;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start ()
    {
        MovementPlaneMesh = MovementPlane.GetComponent<MeshCollider>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(SelectedConstructionPart)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (MovementPlaneMesh.Raycast(ray, out raycastHit, 10000))
            {
                Vector3 hitPoint = raycastHit.point;
                hitPoint.y = 0;
                hitPoint.x = Mathf.Round(hitPoint.x);
                hitPoint.z = Mathf.Round(hitPoint.z);
                //Debug.Log(hitPoint);
                SelectedConstructionPart.transform.position = hitPoint;
            }
        }
	}

    public void SelectPart ( int partId )
    {
        if(partId == 0)
        {
            Debug.Log("Tubes selected");
            SelectedConstructionPart = Tube4Construction;
            SelectedConstructionPart.SetActive(true);
        }
    }

    public Direction OppositeDir(Direction dir)
    {
        if (dir == Direction.BACK) return Direction.FRONT;
        if (dir == Direction.FRONT) return Direction.BACK;
        if (dir == Direction.BOTTOM) return Direction.TOP;
        if (dir == Direction.TOP) return Direction.BOTTOM;
        if (dir == Direction.RIGHT) return Direction.LEFT;
        return Direction.RIGHT;
    }
    
    public Vector3 DirectionIncrement(Direction dir)
    {
        if (dir == Direction.BACK) return new Vector3(0, 0, -1);
        if (dir == Direction.FRONT) return new Vector3(0, 0, 1);
        if (dir == Direction.BOTTOM) return new Vector3(0, -1, 0);
        if (dir == Direction.TOP) return new Vector3(0, 1, 0);
        if (dir == Direction.RIGHT) return new Vector3(1, 0, 0);
        return new Vector3(-1, 0, 0);
    }
}
