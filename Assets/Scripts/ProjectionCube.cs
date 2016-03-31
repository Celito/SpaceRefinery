using UnityEngine;
using System.Collections;

public class ProjectionCube : MonoBehaviour
{
    public delegate void MouseEnteredProjectionAction(int projectionId);
    public delegate void MouseExitedProjectionAction(int projectionId);
    public delegate void MouseReleasedProjectionAction(int projectionId);
    public event MouseEnteredProjectionAction OnMouseEntered;
    public event MouseExitedProjectionAction OnMouseExited;
    public event MouseReleasedProjectionAction OnMouseReleased;

    public Material hiddenMaterial;
    public Material highlightMaterial;

    private Renderer _renderer;
    private int _projectionId;

	// Use this for initialization
	void Start ()
    {
        _renderer = GetComponent<Renderer>();
	}

    void OnMouseEnter()
    {
        if (OnMouseEntered != null) OnMouseEntered(_projectionId); 
    }

    void OnMouseExit()
    {
        if (OnMouseExited != null) OnMouseExited(_projectionId);
    }

    void OnMouseUp()
    {
        if (OnMouseReleased != null) OnMouseReleased(_projectionId); 
    }

    //void OnCollisionEnter(Collision col)
    //{
    //    Debug.Log(name + " colide with " + col.gameObject.name);
    //}

    public void SetProjectionId(int projectionId)
    {
        _projectionId = projectionId;
    }

    public void Highlight()
    {
        _renderer.material = highlightMaterial;
    }

    public void RemoveHighlight()
    {
        _renderer.material = hiddenMaterial;
    }
}
