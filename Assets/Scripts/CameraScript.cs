using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 0.2f;
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float maxZoom = 5f;
    [SerializeField] private float minZoom = 0.8f;
    [SerializeField] private float defaultZoom = 2f;
    [SerializeField] private float defaultCommandCameraSize = 2.5f;
    [SerializeField] private float freeCameraSpeed = 7f;
    [SerializeField] private Transform cameraSubject;
    [SerializeField] private Vector3 pivotOffset = Vector3.zero;
    [SerializeField] private float commandCameraHeight;

    private InputAction lookAction;
    private InputAction zoomAction;
    private InputAction freeCameraSwitchAction;
    private InputAction commandCameraSwitchAction;
    private InputAction moveAction;
    private float zoom;

    private CameraMode currentCameraMode = CameraMode.ORBITAL;
    private CameraMode beforeCommandCameraMode = CameraMode.ORBITAL;
    private bool commandCameraEnabled = false;

    private Camera thisCamera;

    private void Awake()
    {
        Cursor.visible = false;
        zoom = defaultZoom;
        Cursor.lockState = CursorLockMode.Locked;

        lookAction = InputSystem.actions.FindAction("Look");
        zoomAction = InputSystem.actions.FindAction("Zoom");
        freeCameraSwitchAction = InputSystem.actions.FindAction("FreeCameraSwitch");
        commandCameraSwitchAction = InputSystem.actions.FindAction("commandCameraSwitch");
        moveAction = InputSystem.actions.FindAction("Move");

        thisCamera = transform.GetComponent<Camera>();
    }

    private void Update()
    {
        HandleCameraModeSwtich();
        if (currentCameraMode == CameraMode.FREE)
        {
            HandleFreeCamera();
        }
        else if (currentCameraMode == CameraMode.ORBITAL)
        {
            HandleOrbitalCamera();
        }
        else if (currentCameraMode == CameraMode.COMMAND)
        {
            if (!commandCameraEnabled)
            {
                SwitchCommandCamera();
            }
            HandleCommandCamera();
        }
    }
    
    public CameraMode GetCameraMode()
    {
        return currentCameraMode;
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

    private void HandleOrbitalCamera()
    {
        zoom = GetNewZoom();
        Vector3 pivotWithOffset = cameraSubject.position + pivotOffset;

        transform.LookAt(pivotWithOffset);
        transform.eulerAngles = GetNewEulerRotation();
        transform.position = pivotWithOffset - transform.forward * zoom;
    }

    private void HandleCommandCamera()
    {
        Vector2 moveRes = moveAction.ReadValue<Vector2>();
        float horizontalInput = moveRes.x;
        float verticalInput = moveRes.y;

        transform.position += transform.up * verticalInput * freeCameraSpeed * Time.deltaTime;
        transform.position += transform.right * horizontalInput * freeCameraSpeed * Time.deltaTime;
        zoom = GetNewZoom();
        thisCamera.orthographicSize = defaultCommandCameraSize * zoom;

        
    }
    
    private void SwitchCommandCamera()
    {
        commandCameraEnabled = !commandCameraEnabled;

        if (commandCameraEnabled)
        {
            beforeCommandCameraMode = currentCameraMode;
            currentCameraMode = CameraMode.COMMAND;
            transform.position = transform.position + new Vector3(0, commandCameraHeight, 0);
            transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            thisCamera.orthographic = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else
        {
            thisCamera.orthographic = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
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
        bool freeCameraSwitchPressed = freeCameraSwitchAction.triggered && freeCameraSwitchAction.ReadValue<float>() > 0;
        bool commandCameraSwitchPressed = commandCameraSwitchAction.triggered && commandCameraSwitchAction.ReadValue<float>() > 0;

        if (commandCameraSwitchPressed)
        {
            if (currentCameraMode != CameraMode.COMMAND)
            {
                SwitchCommandCamera();
            }
            else
            {
                SwitchCommandCamera();
                currentCameraMode = beforeCommandCameraMode;
            }

        }
        else if (freeCameraSwitchPressed)
        {
            if (commandCameraEnabled)
            {
                SwitchCommandCamera();
            }
            if (currentCameraMode != CameraMode.FREE)
            {
                currentCameraMode = CameraMode.FREE;
            }
            else
            {
                currentCameraMode = CameraMode.ORBITAL;
            }
        }
    }
    
    private float GetNewZoom()
    {
        return Mathf.Clamp(zoom - zoomAction.ReadValue<Vector2>().y / zoomSpeed, minZoom, maxZoom);
    }
}