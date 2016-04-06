using UnityEngine;
using System.Collections;

public class PointOfInterest : MonoBehaviour
{
    public delegate void SelectAction();
    public delegate void DeselectAction();
    public event SelectAction OnSelected;
    public event DeselectAction OnDeselected;

    private float _sensitivity;
    private Vector3 _mouseReference;
    private Vector3 _mouseOffset;
    private Vector3 _rotation;
    private bool _isRotatingAround;
    private bool _isSelected;
    private bool _isMouseOver;

    private Transform _cameraBoomTransform;

    void Start()
    {
        _sensitivity = 0.4f;
        _rotation = Vector3.zero;
    }

    void Update()
    {
        if (_isSelected && _isRotatingAround)
        {
            // offset
            _mouseOffset = (Input.mousePosition - _mouseReference);

            // apply rotation
            _rotation.y = _mouseOffset.x * _sensitivity;
            _rotation.x = -_mouseOffset.y * _sensitivity;

            // rotate
            _cameraBoomTransform.Rotate(_rotation);

            // store mouse
            _mouseReference = Input.mousePosition;
        }
    }

    void OnMouseDown()
    {
        // TODO: pivot the camera when clicked with the mouse 3rd button
        if(_isSelected)
        {
            // rotating flag
            _isRotatingAround = true;

            // store mouse
            _mouseReference = Input.mousePosition;

            GUIManager.instance.changeCursor(GUIManager.DRAGING_CURSOR);
        }
        else
        {
            //MainCamera.instance.SetNewPOI(this);
        }
    }

    void OnMouseUp()
    {
        if(_isRotatingAround)
        {
            if(_isMouseOver)
            {
                GUIManager.instance.changeCursor(GUIManager.DRAGBLE_CURSOR);
            }
            else
            {
                GUIManager.instance.changeCursor(GUIManager.NORMAL_CURSOR);
            }

            _isRotatingAround = false;
        }
    }

    public void Select()
    {
        _cameraBoomTransform = transform.FindChild("CameraBoom");
        if (_cameraBoomTransform)
        {
            _isSelected = true;
            if(OnSelected != null)
            {
                OnSelected();
            }
        }
    }

    public void Deselect()
    {
        _isSelected = false;
        if(OnDeselected != null)
        {
            OnDeselected();
        }
    }

    // TODO: Move those to a proper class

    void OnMouseEnter()
    {
        _isMouseOver = true;
        if(_isSelected && !_isRotatingAround)
        {
            GUIManager.instance.changeCursor(GUIManager.DRAGBLE_CURSOR);
        }
        else if(!_isSelected && !_isRotatingAround)
        {
            GUIManager.instance.changeCursor(GUIManager.SELECTABLE_CURSOR);
        }
    }

    void OnMouseExit()
    {
        _isMouseOver = false;
        if (!_isRotatingAround)
        {
            GUIManager.instance.changeCursor(GUIManager.NORMAL_CURSOR);
        }
    }
}
