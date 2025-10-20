using UnityEngine;
using UnityEngine.InputSystem;

public class FoxyController : MonoBehaviour
{
    [SerializeField] private CameraScript cameraScript;

    private FoxyControlScript foxyControlScript;

    private InputAction moveAction;
    private InputAction brakeAction;

    private float verticalInput;
    private float horizontalInput;
    private bool isBraking;


    private void Awake()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        brakeAction = InputSystem.actions.FindAction("Jump");
        foxyControlScript = transform.GetComponent<FoxyControlScript>();
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
        foxyControlScript.SetInput(verticalInput, horizontalInput);
        if (isBraking)
        {
            foxyControlScript.Brake();
        }
    }
}
