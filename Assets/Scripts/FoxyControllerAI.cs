using UnityEngine;

public class FoxyControllerAI : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] private Vector3 targetPosition = new Vector3(5, 0, 5);

    [SerializeField] private float turnaroundDistance = 3f;
    [SerializeField] private float targetReachedDistance = 1f;
    [SerializeField] private float brakeBeforeTargetDistance = 2f;
    [SerializeField] private float minSpeedToBrake = 10f;
    [SerializeField] private float minAngleToTurn = 5f;
    [SerializeField] private float speed = 0.5f;

    private FoxyControlScript foxyControlScript;

    void Awake()
    {
        foxyControlScript = transform.GetComponent<FoxyControlScript>();
    }

    private void Update()
    {
        Vector3 forward = transform.forward * -1;
        targetPosition = target.position;
        float speedInput;
        float turnInput = 0f;

        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        float dotProduct = Vector3.Dot(forward, directionToTarget);

        if (distanceToTarget < targetReachedDistance)
        {
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
        

        foxyControlScript.SetInput(speedInput, turnInput);
    }
}
