using UnityEngine;



public class Product : MonoBehaviour
{
    private Processes _availableProcesses;
    private Vector3 _targetVector;
    private Vector3 _currentPosition;
    private bool _subscribedToMove;

    public void SetTarget(Vector3 position)
    {
        _targetVector = position - transform.position;
        _currentPosition = transform.position;
        if(!_subscribedToMove)
        {
            MovementManager.instance.movementUpdate += MovementUpdate;
            _subscribedToMove = true;
        }
    }

    private void MovementUpdate(float movementProgress)
    {
        // Rounding progress threshold at 95%
        if(movementProgress > 0.95)
        {
            transform.position = _currentPosition + _targetVector;
            _currentPosition = transform.position;
            MovementManager.instance.movementUpdate -= MovementUpdate;
            _subscribedToMove = false;
        }
        else
        {
            transform.position = _currentPosition + (_targetVector * movementProgress);
        }
    }
}
