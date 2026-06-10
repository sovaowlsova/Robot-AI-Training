using UnityEngine;
using UnityEngine.InputSystem;

public class FoxyControllerManual : ManualController
{
    [SerializeField] private CameraScript cameraScript;
    private IControllable controlScript;

    private InputAction moveAction;
    private InputAction brakeAction;

    private float verticalInput;
    private float horizontalInput;
    private bool isBraking;


    private void Awake()
    {
        controlScript = transform.GetComponent<IControllable>();
        moveAction = InputSystem.actions.FindAction("Move");
        brakeAction = InputSystem.actions.FindAction("Jump");
    }

    private void Update()
    {
        if (cameraScript.GetCameraMode() == CameraMode.COMMAND)
        {
            return;
        }
        GetInput();
        SetInput();
    }

    private void GetInput()
    {
        Vector2 moveResult = moveAction.ReadValue<Vector2>();
        verticalInput = moveResult.y;
        horizontalInput = moveResult.x;
        isBraking = brakeAction.ReadValue<float>() > 0;
    }

    private void SetInput()
    {
        controlScript.SetInput(verticalInput, horizontalInput);
        if (isBraking)
        {
            controlScript.Brake();
        }
    }
}
