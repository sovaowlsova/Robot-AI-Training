using System;
using System.Collections.Generic;
using UnityEngine;

public class SonarScript : MonoBehaviour
{
    [SerializeField] private Transform transmitter;
    [SerializeField] private Transform receiver;

    [SerializeField] private int precision = 4;

    [SerializeField] private float maxDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private float measuringAngleEuler;

    private float closestObject;
    private float raycastSphereRadius;

    private Vector3 transmitterBackwards;
    private Vector3 receiverBackwards;

    private void Awake()
    {
        closestObject = maxDistance;

        float halvedAngle = measuringAngleEuler / 2f * ((float)Math.PI / 180f);
        raycastSphereRadius = Mathf.Tan(halvedAngle) * maxDistance * 2f;
    }

    private void FixedUpdate()
    {
        transmitterBackwards = transmitter.forward * -1;
        receiverBackwards = receiver.forward * -1;
        closestObject = maxDistance;
        FindClosestObject();
    }

    public float GetDistance()
    {
        return closestObject;
    }

    private void FindClosestObject()
    {
        Vector3 origin = transmitter.position - transmitterBackwards * raycastSphereRadius;

        List<RaycastHit> allHits = new List<RaycastHit>();

        for (int i = 1; i <= precision; i++)
        {
           RaycastHit[] sphereHits = Physics.SphereCastAll(origin, raycastSphereRadius / i, transmitterBackwards, maxDistance);
           allHits.AddRange(sphereHits);
        }

        foreach (RaycastHit hit in allHits) 
        {
            float distance = Vector3.Distance(receiver.position, hit.point);
            if (ValidateHit(hit) && distance < closestObject) 
            {
                closestObject = distance;
            }
        }
    }
    
    private bool ValidateHit(RaycastHit hit)
    {
        RaycastHit hitTransmitter;
        RaycastHit hitReceiver;

        Vector3 directionToTransmitter = (transmitter.position - hit.point).normalized;
        Vector3 directionToReceiver = (receiver.position - hit.point).normalized;

        // Did raycast hit anything?
        if (!Physics.Raycast(hit.point, directionToTransmitter, out hitTransmitter, maxDistance) ||
            !Physics.Raycast(hit.point, directionToReceiver, out hitReceiver, maxDistance))
        {
            return false;
        }

         // Check if raycast hit the sensor
        if (hitTransmitter.collider.transform != transmitter ||
            hitReceiver.collider.transform != receiver)
        {
            return false;
        }


        // Check if object is far enough
        if (hitTransmitter.distance < minDistance || hitReceiver.distance < minDistance)
        {
            return false;
        }

        // Check angle
        if (Vector3.Angle(transmitterBackwards, directionToTransmitter * -1) > measuringAngleEuler &&
            Vector3.Angle(receiverBackwards, directionToReceiver * -1) > measuringAngleEuler)
        {
            return false;
        }

        return true;
    }
}
