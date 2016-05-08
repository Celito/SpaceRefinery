using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public enum Direction { BOTTOM = 1, LEFT, BACK, FRONT, RIGHT, TOP };

    public static GridManager instance = null;

    public float cubesSize = 1.0f;

    public GameObject TubesTip;
    public GameObject ProjectionCube;
    public GameObject TubeBodyStraight;
    public GameObject TubeBodyCurve;

    public GameObject MovementPlaneGameObj;
    public MeshCollider MovementPlaneMeshCollider;
    
    public GameObject Tube4;

    public GameObject Tube4Blueprint;
    public GameObject ReservoirBlueprint;

    public GameObject SelectedConstructionPart;

    public Material BuildingEnabledMaterial;
    public Material BuildingBlockedMaterial;

    private int _currPartId = -1;

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
        MovementPlaneMeshCollider = MovementPlaneGameObj.GetComponent<MeshCollider>();
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(SelectedConstructionPart)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (MovementPlaneMeshCollider.Raycast(ray, out raycastHit, 10000))
            {
                Vector3 hitPoint = raycastHit.point;
                hitPoint.x = Mathf.Round(hitPoint.x);
                hitPoint.z = Mathf.Round(hitPoint.z);
                hitPoint.y = MainCamera.instance.cameraBoom.transform.position.y;
                SelectedConstructionPart.SendMessage("MoveBlueprintTo", hitPoint);
            }
        }
        
        // Move up and down with the mouse wheel;
        Vector3 upNDownVec = Vector3.up * (Input.GetAxis("UpNDown") > 0 ? 1 : Input.GetAxis("UpNDown") < 0 ? -1 : 0);
        MovementPlaneGameObj.transform.Translate(upNDownVec);
        MainCamera.instance.cameraBoom.transform.Translate(upNDownVec, Space.World);

        // Move arround the grid with the vertical and horizontal axis
        Vector3 planeNormal = MovementPlaneGameObj.transform.up;
        Vector3 cameraForward = MainCamera.instance.transform.forward;
        Vector3 cameraRight = MainCamera.instance.transform.right;
        Vector3 movementForward = ProjectVectorOntoPlane(cameraForward, planeNormal).normalized * Input.GetAxis("Vertical");
        Vector3 movementSideways = ProjectVectorOntoPlane(cameraRight, planeNormal).normalized *Input.GetAxis("Horizontal");
        Vector3 cameraPlaneMovement = (movementForward + movementSideways) * Time.deltaTime * MainCamera.instance.dragVelocity;
        MainCamera.instance.cameraBoom.transform.Translate(cameraPlaneMovement, Space.World);
    }

    private Vector3 ProjectVectorOntoPlane(Vector3 vec, Vector3 planeNormal)
    {
        var distance = -Vector3.Dot(planeNormal.normalized, vec);
        return vec + planeNormal.normalized * distance;
    }

    public void SelectPart ( int partId )
    {
        if(partId != _currPartId)
        {
            if(SelectedConstructionPart)
            {
                SelectedConstructionPart.SetActive(false);
                SelectedConstructionPart = null;
            }
            _currPartId = partId;
        }
        if(partId == 0)
        {
            Debug.Log("Tubes selected");
            SelectedConstructionPart = Tube4Blueprint;
            SelectedConstructionPart.SetActive(true);
        }
        else if(partId == 1)
        {
            Debug.Log("Reservoir selected");
            SelectedConstructionPart = ReservoirBlueprint;
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
    
    public static Vector3 DirectionIncrement(Direction dir)
    {
        if (dir == Direction.BACK) return new Vector3(0, 0, -1);
        if (dir == Direction.FRONT) return new Vector3(0, 0, 1);
        if (dir == Direction.BOTTOM) return new Vector3(0, -1, 0);
        if (dir == Direction.TOP) return new Vector3(0, 1, 0);
        if (dir == Direction.RIGHT) return new Vector3(1, 0, 0);
        return new Vector3(-1, 0, 0);
    }
}
