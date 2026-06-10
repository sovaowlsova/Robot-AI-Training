using UnityEngine;

public class WheeltecControlScript : MonoBehaviour, IControllable
{
    [SerializeField] private float motorForce = 100f;
    [SerializeField] private float brakeForce = 1000f;
    [SerializeField] private float maxSteerAngle = 15f;
    [SerializeField] private float steerSpeed = 20f;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [SerializeField] private WheelCollider frontLeftWheelColldier;
    [SerializeField] private WheelCollider frontRightWheelColldier;
    [SerializeField] private WheelCollider rearLeftWheelColldier;
    [SerializeField] private WheelCollider rearRightWheelColldier;

    private float verticalInput;
    private float horizontalInput;
    private bool isBraking;

    private float currentSteerAngle;
    private float currentBrakingForce;

    private float epsilon = 1e-3f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        isBraking = false;
        verticalInput = 0;
        horizontalInput = 0;
    }

    public void SetInput(float speed, float turn)
    {
        verticalInput = speed;
        horizontalInput = turn;
    }

    public void Brake()
    {
        isBraking = true;
    }

    public float GetSpeed()
    {
        return rb.linearVelocity.magnitude;
    }

    private void HandleMotor()
    {
        if (verticalInput < 0)
        {
            verticalInput = -1;
        } else if (verticalInput > 0)
        {
            verticalInput = 1;
        }
        float newTorque = -verticalInput * motorForce;
        frontLeftWheelColldier.motorTorque = frontRightWheelColldier.motorTorque = newTorque;

        currentBrakingForce = isBraking ? brakeForce : 0f;
        applyBraking();
    }

    private void applyBraking()
    {
        frontLeftWheelColldier.brakeTorque = currentBrakingForce;
        frontRightWheelColldier.brakeTorque = currentBrakingForce;
        rearLeftWheelColldier.brakeTorque = currentBrakingForce;
        rearRightWheelColldier.brakeTorque = currentBrakingForce;
    }

    private void HandleSteering()
    {
        if (horizontalInput > 0)
        {
            currentSteerAngle = Mathf.Clamp(currentSteerAngle + steerSpeed * Time.deltaTime, -maxSteerAngle, maxSteerAngle);
        }
        else if (horizontalInput < 0)
        {
            currentSteerAngle = Mathf.Clamp(currentSteerAngle - steerSpeed * Time.deltaTime, -maxSteerAngle, maxSteerAngle);
        }
        else
        {
            if (currentSteerAngle > epsilon)
            {
                currentSteerAngle = Mathf.Clamp(currentSteerAngle - steerSpeed * Time.deltaTime, 0, maxSteerAngle);
            }
            else if (currentSteerAngle < epsilon)
            {
                currentSteerAngle = Mathf.Clamp(currentSteerAngle + steerSpeed * Time.deltaTime, -maxSteerAngle, 0);
            }
        }
        frontLeftWheelColldier.steerAngle = frontRightWheelColldier.steerAngle = currentSteerAngle;
    }

    private void UpdateWheelTransform(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private void UpdateWheels()
    {
        UpdateWheelTransform(frontLeftWheelColldier, frontLeftWheelTransform);
        UpdateWheelTransform(frontRightWheelColldier, frontRightWheelTransform);
        UpdateWheelTransform(rearLeftWheelColldier, rearLeftWheelTransform);
        UpdateWheelTransform(rearRightWheelColldier, rearRightWheelTransform);
    }
}
