using UnityEngine;
using UnityEngine.InputSystem;

public class FoxyController : MonoBehaviour
{
    [SerializeField] private float motorForce = 100f;
    [SerializeField] private float brakeForce = 1000f;
    [SerializeField] private float maxSteerAngle = 15f;
    [SerializeField] private float steerSpeed = 20f;

    [SerializeField] private float warningDistance = 1;
    [SerializeField] private float stopDistance = 0.2f;

    [SerializeField] private LampScript warningLamp;
    [SerializeField] private LampScript stopLamp;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [SerializeField] private WheelCollider frontLeftWheelColldier;
    [SerializeField] private WheelCollider frontRighttWheelColldier;
    [SerializeField] private WheelCollider rearLeftWheelColldier;
    [SerializeField] private WheelCollider rearRighttWheelColldier;

    [SerializeField] private SonarScript sonar;

    private InputAction moveAction;
    private InputAction brakeAction;

    private float verticalInput;
    private float horizontalInput;
    private bool isBraking;

    private float currentSteerAngle;
    private float currentBrakingForce;

    private float epsilon = 1e-3f;

    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        brakeAction = InputSystem.actions.FindAction("Jump");
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        Vector2 moveResult = moveAction.ReadValue<Vector2>();
        verticalInput = moveResult.y;
        horizontalInput = moveResult.x;
        
        isBraking = brakeAction.ReadValue<float>() > epsilon ? true : false;
        HandleSonar();
    }

    private void HandleSonar()
    {
        if (sonar.GetDistance() < warningDistance)
        {
            warningLamp.Toggle(true);
        }
        else
        {
            warningLamp.Toggle(false);
        }

        if (sonar.GetDistance() < stopDistance)
        {
            stopLamp.Toggle(true);

            if (verticalInput >= 0)
            {
                isBraking = true;
            }
        }
        else
        {
            stopLamp.Toggle(false);
        }
    }

    private void HandleMotor()
    {
        float newTorque = -verticalInput * motorForce;
        frontLeftWheelColldier.motorTorque = frontRighttWheelColldier.motorTorque = newTorque;

        currentBrakingForce = isBraking ? brakeForce : 0f;
        applyBraking();
    }

    private void applyBraking()
    {
        frontLeftWheelColldier.brakeTorque = currentBrakingForce;
        frontRighttWheelColldier.brakeTorque = currentBrakingForce;
        rearLeftWheelColldier.brakeTorque = currentBrakingForce;
        rearRighttWheelColldier.brakeTorque = currentBrakingForce;
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
        frontLeftWheelColldier.steerAngle = frontRighttWheelColldier.steerAngle = currentSteerAngle;
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
        UpdateWheelTransform(frontRighttWheelColldier, frontRightWheelTransform);
        UpdateWheelTransform(rearLeftWheelColldier, rearLeftWheelTransform);
        UpdateWheelTransform(rearRighttWheelColldier, rearRightWheelTransform);
    }
}
