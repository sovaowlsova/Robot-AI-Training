using System.Collections.Generic;
using UnityEngine;

public class FoxyControllerAI : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] private CommandScript user;

    [SerializeField] private float turnaroundDistance = 3f;
    [SerializeField] private float targetReachedDistance = 1f;
    [SerializeField] private float brakeBeforeTargetDistance = 2f;
    [SerializeField] private float minSpeedToBrake = 10f;
    [SerializeField] private float minAngleToTurn = 5f;
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float loopingPreventDistance = 1f;
    [SerializeField] private float loopingPreventDotProduct = 0.5f;

    private FoxyControlScript foxyControlScript;

    private void Awake()
    {
        foxyControlScript = transform.GetComponent<FoxyControlScript>();
    }

    private void Update()
    {
        List <Transform> movePoints = user.GetMovePoints();
        Vector3 targetPosition;
    
        if (movePoints.Count > 0)
        {
            targetPosition = movePoints[0].position;
        } else
        {
            foxyControlScript.Brake();
            return;
        }

        Vector3 forward = transform.forward * -1;
        float speedInput;
        float turnInput = 0f;

        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        float dotProduct = Vector3.Dot(forward, directionToTarget);

        if (distanceToTarget < targetReachedDistance)
        {
            user.RemoveMovePoint(movePoints[0]);
            foxyControlScript.Brake();
            foxyControlScript.SetInput(0f, 0f);
            return;
        }

        if (dotProduct > 0)
        {
            speedInput = speed;
        }
        else
        {
            speedInput = distanceToTarget > turnaroundDistance ? speed : -speed;
        }

        if (brakeBeforeTargetDistance > distanceToTarget && foxyControlScript.GetSpeed() > minSpeedToBrake)
        {
            speedInput *= -1;
        }
        
        float angleToTarget = Vector3.SignedAngle(forward, directionToTarget, Vector3.up);
        if (Mathf.Abs(angleToTarget) > minAngleToTurn)
        {
            turnInput = angleToTarget > 0 ? 1f : -1f;
        }
        
        // Check if we are looping around a point and try to fix it
        if (distanceToTarget < loopingPreventDistance && Mathf.Abs(dotProduct) < loopingPreventDotProduct)
        {
            speedInput = dotProduct > 0 ? -speed : speed;
            turnInput = 0;
        }

        foxyControlScript.SetInput(speedInput, turnInput);
    }
}
