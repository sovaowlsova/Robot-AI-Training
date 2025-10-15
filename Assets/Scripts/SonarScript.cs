using System.Collections.Generic;
using UnityEngine;

public class SonarScript : MonoBehaviour
{
    [SerializeField] private Transform transmitter;
    [SerializeField] private Transform receiver;

    [SerializeField] private float maxDistance;
    [SerializeField] private float minDistance;

    private List<Collider> collidersInside = new List<Collider>();
    private int detectableColliders = 0;
    private float closestObject;


    private void FixedUpdate()
    {
        closestObject = maxDistance;
        
        foreach (Collider collider in collidersInside)
        {
            if (ValidateDetection(collider))
            {
                if (detectableColliders < collidersInside.Count)
                {
                    detectableColliders++;
                }
            }
            else
            {
                detectableColliders--;
            }
        }
    }

    public float GetDistance()
    {
        return closestObject;
    }

    public bool IsTriggered()
    {
        return detectableColliders > 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ValidateDetection(other))
        {
            collidersInside.Add(other);
            detectableColliders++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        collidersInside.Remove(other);
        detectableColliders--;
    }

    private bool ValidateDetection(Collider other)
    {
        RaycastHit hitTransmitter;
        RaycastHit hitReceiver;

        Vector3 closestPointToTransmitter = other.ClosestPoint(transmitter.position);
        Vector3 closestPointToReceiver = other.ClosestPoint(receiver.position);

        Vector3 directionToTransmitter = (transmitter.position - closestPointToTransmitter).normalized;
        Vector3 directionToReceiver = (receiver.position - closestPointToReceiver).normalized;

        // Did raycast hit anything?
        if (!Physics.Raycast(closestPointToTransmitter, directionToTransmitter, out hitTransmitter, maxDistance) ||
            !Physics.Raycast(closestPointToReceiver, directionToReceiver, out hitReceiver, maxDistance))
        {
            return false;
        }

         // Check if raycast hit the sensor
        if (hitTransmitter.collider.transform.parent != transform.parent ||
            hitReceiver.collider.transform.parent != transform.parent)
        {
            return false;
        }


        // Check if object is far enough
        if (hitTransmitter.distance < minDistance || hitReceiver.distance < minDistance)
        {
            return false;
        }

        if (hitReceiver.distance < closestObject)
        {
            closestObject = hitReceiver.distance;
        }

        return true;
    }
}
