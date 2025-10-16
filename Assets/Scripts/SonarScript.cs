using System;
using System.Collections.Generic;
using UnityEngine;

public class SonarScript : MonoBehaviour
{
    [SerializeField] private Transform transmitter;
    [SerializeField] private Transform receiver;

    [SerializeField] private float maxDistance;
    [SerializeField] private float minDistance;
    [SerializeField] private float measuringAngleEuler;

    private float closestObject;
    private float raycastSphereRadius;
    private float raycastDistance;

    private Vector3 transmitterBackwards;
    private Vector3 receiverBackwards;
    private Vector3 origin;

    private void Awake()
    {
        closestObject = maxDistance;

        float halvedAngle = measuringAngleEuler / 2f * ((float)Math.PI / 180f);
        raycastSphereRadius = Mathf.Tan(halvedAngle) * maxDistance * 2f;
        raycastDistance = maxDistance - raycastSphereRadius;
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
        origin = transmitter.position + transmitterBackwards * raycastSphereRadius;

        // SphereCastAll returns (0, 0, 0) if there's an object inside the first sphere
        // Yes. Unity design is dum-dum so we have to do this absolute mess
        List<RaycastHit> allHits = new List<RaycastHit>();

        RaycastHit[] sphereHits = Physics.SphereCastAll(origin, raycastSphereRadius, transmitterBackwards, raycastDistance);
        allHits.AddRange(sphereHits);

        Collider[] overlappedFirst = Physics.OverlapSphere(origin, raycastSphereRadius);
        foreach (Collider collider in overlappedFirst)
        {
            Vector3 closestPoint = collider.ClosestPoint(receiver.transform.position);
            RaycastHit hit;
            if (Physics.Raycast(closestPoint, (closestPoint - receiver.position).normalized, out hit, maxDistance))
            {
                allHits.Add(hit);
            }
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
        if (hitTransmitter.collider.transform.parent != transform ||
            hitReceiver.collider.transform.parent != transform)
        {
            return false;
        }


        // Check if object is far enough
        if (hitTransmitter.distance < minDistance || hitReceiver.distance < minDistance)
        {
            return false;
        }

        // Check angle
        if (Vector3.Angle(transmitterBackwards, directionToTransmitter * -1) > measuringAngleEuler ||
            Vector3.Angle(receiverBackwards, directionToReceiver * -1) > measuringAngleEuler)
        {
            return false;
        }

        return true;
    }
}
