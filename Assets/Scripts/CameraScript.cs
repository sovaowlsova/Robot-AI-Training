using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float turnSpeed;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float maxZoom;
    [SerializeField] private float minZoom;
    [SerializeField] private float defaultZoom;
    [SerializeField] private float freeCameraSpeed;
    [SerializeField] private Transform cameraObject;
    [SerializeField] private Vector3 pivotOffset;

    private InputAction lookAction;
    private InputAction zoomAction;
    private InputAction switchCameraModeAction;
    private InputAction moveAction;
    private float zoom;

    private bool isCameraLocked = true;

    private FoxyController foxyController;
    private FoxyController foxyControllerAI;

    private void Awake()
    {
        Cursor.visible = false;
        zoom = defaultZoom;
        Cursor.lockState = CursorLockMode.Locked;
        lookAction = InputSystem.actions.FindAction("Look");
        zoomAction = InputSystem.actions.FindAction("Zoom");
        switchCameraModeAction = InputSystem.actions.FindAction("SwitchCameraMode");
        moveAction = InputSystem.actions.FindAction("Move");
        foxyController = cameraObject.GetComponent<FoxyController>();
    }

    private void Update()
    {
        HandleCameraModeSwtich();
        if (isCameraLocked)
        {
            HandleLockedCamera();
        }
        else
        {
            HandleFreeCamera();
        }
    }

    private void HandleFreeCamera()
    {
        Vector2 moveRes = moveAction.ReadValue<Vector2>();
        float horizontalInput = moveRes.x;
        float verticalInput = moveRes.y;

        transform.position += transform.forward * verticalInput * freeCameraSpeed * Time.deltaTime;
        transform.position += transform.right * horizontalInput * freeCameraSpeed * Time.deltaTime;
        transform.eulerAngles = GetNewEulerRotation();
    }

    private void HandleLockedCamera()
    {
        zoom = Mathf.Clamp(zoom - zoomAction.ReadValue<Vector2>().y / zoomSpeed, minZoom, maxZoom);
        Vector3 pivotWithOffset = cameraObject.position + pivotOffset;

        
        transform.LookAt(pivotWithOffset);
        transform.eulerAngles = GetNewEulerRotation();
        transform.position = pivotWithOffset - transform.forward * zoom;
    }

    private Vector3 GetNewEulerRotation()
    {
        Vector2 lookRes = lookAction.ReadValue<Vector2>() * turnSpeed;
        Vector3 newAngle = transform.eulerAngles + new Vector3(-lookRes.y, lookRes.x, 0);
        if (newAngle.x > 180 && newAngle.x < 271)
        {
            newAngle.x = 271;
        }
        else if (newAngle.x > 89 && newAngle.x < 180)
        {
            newAngle.x = 89;
        }
        return newAngle;
    }

    private void HandleCameraModeSwtich()
    {
        bool isSwitchKeyPressed = switchCameraModeAction.triggered && switchCameraModeAction.ReadValue<float>() > 0;
        if (isSwitchKeyPressed)
        {
            isCameraLocked = !isCameraLocked;

        }
    }
}
