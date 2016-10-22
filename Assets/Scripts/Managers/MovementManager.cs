using UnityEngine;
using System;

public class MovementManager : MonoBehaviour
{
    public static MovementManager instance { get { return _instance; } }
    private static MovementManager _instance;

    /// <summary>
    /// Defines how much of the tickFrequency is spent doing the movement, that creates the move 
    /// and stop effect for the products been conveyed throgh the machine
    /// </summary>
    public float movementPortion = 0.8f;

    public float movementCurvature = 0.5f;

    public event Action<float> movementUpdate;

    private float currProgress;

    void Awake()
    {
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        // The movement progress value will be defined by the portion of time pass form the last 
        // tick times the portion of time the products should be moving;
        // TODO: edit this value to make a "curvy" movement progress
        float movementProgress = (float)Math.Pow((UpdateManager.instance.timePassFromLastTick / 
            (UpdateManager.instance.tickFrequency * movementPortion)), movementCurvature);
        currProgress = movementProgress;
        if (movementUpdate != null) movementUpdate(movementProgress);
    }
}
